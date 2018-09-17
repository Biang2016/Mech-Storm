// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/LandTile"
{
	Properties
	{
		_ForegroundSample("Foreground", 2D) = "white" {}
		_BackgroundSample("Background", 2D) = "white" {}
		_MaskSample("Mask", 2D) = "red" {}

		_Tiling("Tiling", Float) = 1
	}
	SubShader
		{
			Tags{ "QUEUE" = "Background+1""IGNOREPROJECTOR" = "true" "RenderType" = "Opaque" }
			LOD 100

			Pass
			{
				ZWrite Off

				CGPROGRAM
#pragma vertex vert
#pragma fragment frag
				// make fog work
#pragma multi_compile_fog

#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
					float2 backuv : TEXCOORD1;
					float2 maskuv : TEXCOORD2;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
					float2 uv : TEXCOORD0;
					float3 position : TEXCOORD1;
					float2 maskuv : TEXCOORD2;
					float2 backuv : TEXCOORD3;
					UNITY_FOG_COORDS(3)
				};

				sampler2D _ForegroundSample;
				sampler2D _BackgroundSample;
				sampler2D _MaskSample;
				float _Tiling;

				v2f vert(appdata v)
				{
					v2f o;

					o.vertex = UnityObjectToClipPos(v.vertex);

					float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

					o.uv = v.uv;
					o.maskuv = v.maskuv;
					o.backuv = v.backuv;
					o.position = worldPos;

					UNITY_TRANSFER_FOG(o, o.vertex);
					 
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					float2 uv = i.uv;//i.position.xy * _Tiling;

					float3 fore = tex2D(_ForegroundSample, uv);
					float3 back = tex2D(_BackgroundSample, i.backuv);

					float mask = tex2D(_MaskSample, i.maskuv).b;
					mask = 1 - mask;
					fixed4 col;

					col.rgb = (fore * mask + back * (1 - mask));

					col.a =1;

					// apply fog
					UNITY_APPLY_FOG(i.fogCoord, col);

					return col;
				}
					ENDCG
			}
		}
}
