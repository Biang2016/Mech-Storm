// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/RoadTile"
{
	Properties
	{
		_Foreground("Foreground", 2D) = "white" {}
		_Background("Background", 2D) = "white" {}
	}
		SubShader
	{
		Tags{ "QUEUE" = "Background+9" "IGNOREPROJECTOR" = "true" "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
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
				float4 color : COLOR0;
				float2 uv1 : TEXCOORD1;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float4 color: COLOR0;
			};

			sampler2D _Foreground;
			sampler2D _Background;
			//float4 _Color;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.uv1 = v.uv1;
				o.color = v.color;
				UNITY_TRANSFER_FOG(o, o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_Foreground, i.uv) * 1;
				fixed4 col2 = tex2D(_Background, i.uv1) * 1;
				col = col * col2;
				return col;
			}
			ENDCG
		}
	}
}
