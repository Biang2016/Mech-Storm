// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/coloration_all"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		//_MainMask ("Mask", 2D) = "white" {}
		//_Color ("Color", color) = (1,1,1,1)
		_Hue ("hue", float) = 0
	}
	SubShader
	{
		Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
		LOD 100

		Pass
		{
			Cull off
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
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float4 color: COLOR0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			//sampler2D _MainMask;
			//float4 _MainMask_ST;
			float4 _Color;
			float1 _Hue;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color;
				return o;
			}
			
			float3 RGB2HSV(float3 c)
			{
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
				float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

				float d = q.x - min(q.w, q.y);
				float e = 1.0e-10;
				return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}

			float3 HSV2RGB(float3 c)
			{
				  float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
				  float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
				  return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
			}

			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				//fixed4 col = tex2D(_MainTex, i.uv)*i.color.a;
				fixed4 col = tex2D(_MainTex, i.uv);
				//fixed4 bkcol = col;

				fixed3 col_hsv = RGB2HSV(col);
				col_hsv.r = _Hue;
				col.rgb = HSV2RGB(col_hsv);

				//col = bkcol.a<0.5f&&bkcol.a>0.0f?1.0f:0.0f;
				//col = bkcol.a;
				return col;
			}

			ENDCG
		}
	}
}
