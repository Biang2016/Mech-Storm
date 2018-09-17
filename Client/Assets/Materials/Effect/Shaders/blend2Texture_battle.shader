// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "golden/Blend2Texture_battle"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_AlphaTex("Texture", 2D) = "white" {}
		_Color("Color", color) = (1, 1, 1, 1)
	}
	SubShader
		{
			Tags{ "QUEUE" = "Geometry+2" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" }
			LOD 100
			Pass
			{
				ZWrite Off
				Blend SrcAlpha OneMinusSrcAlpha
				//ZWrite off
				CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float2 uv2: TEXCOORD1;
					float4 vertex : SV_POSITION;
				};

				sampler2D _MainTex;
				sampler2D _AlphaTex;
				float4 _MainTex_ST;
				float4 _AlphaTex_ST;
				float4 _Color;

				v2f vert(appdata v)
				{
					v2f o;
					//vertex offset,
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					o.uv2 = TRANSFORM_TEX(v.uv, _AlphaTex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					// sample the texture
					fixed4 col = tex2D(_MainTex, i.uv);
					col.a = tex2D(_AlphaTex, i.uv2).r;
					col *= _Color;
					return col;
				}
					ENDCG
			}
		}
}
