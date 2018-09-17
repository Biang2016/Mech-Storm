// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/ripple"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_texal("texal", float) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			ZTest Always
			Cull Off 
			//ZWrite Off
			Fog { Mode off }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag2
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _texal;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float texelX = _texal;
				float texelY = texelX;

				float xl = i.uv.x - texelX;	// 左邻点x坐标
				float xr = i.uv.x + texelX;	// 右邻点x坐标
				float yu = i.uv.y - texelY;	// 上邻点y坐标
				float yd = i.uv.y + texelY;	// 下邻点y坐标

				float l = tex2D(_MainTex, float2(xl, i.uv.y)).r;
				float r = tex2D(_MainTex, float2(xr, i.uv.y)).r;
				float u = tex2D(_MainTex, float2(i.uv.x, yu)).r;
				float d = tex2D(_MainTex, float2(i.uv.x, yd)).r;

				float lu = tex2D(_MainTex, float2(xl, yu)).r;
				float ld = tex2D(_MainTex, float2(xl, yd)).r;
				float ru = tex2D(_MainTex, float2(xr, yu)).r;
				float rd = tex2D(_MainTex, float2(xr, yd)).r;

				float dx = -lu - 2*l -ld + ru + 2*r + rd;
				float dy = -lu - 2*u -ru + ld + 2*d + rd;
 				float3 normal = normalize(float3(-dx, -dy, 1.0));

 				normal = normal * 0.5 + 0.5;
 
 				return float4(normal, 1);
				//fixed4 col = tex2D(_MainTex, i.uv);
				//return col;
			}
			fixed4 frag2 (v2f i) : SV_Target
			{
				float texelX = _texal;
				float texelY = texelX;

				float xl = i.uv.x - texelX;	// 左邻点x坐标
				float xr = i.uv.x + texelX;	// 右邻点x坐标
				float yu = i.uv.y - texelY;	// 上邻点y坐标
				float yd = i.uv.y + texelY;	// 下邻点y坐标

				float l = tex2D(_MainTex, float2(xl, i.uv.y)).r;
				float r = tex2D(_MainTex, float2(xr, i.uv.y)).r;
				float u = tex2D(_MainTex, float2(i.uv.x, yu)).r;
				float d = tex2D(_MainTex, float2(i.uv.x, yd)).r;

				float2 color = tex2D(_MainTex,i.uv).rg;
				float per1H = color.r;
				float per2H = color.g;

				float height = 0.5 * (l+r+u+d) - per2H;	// A * (l+r+u+d) + B * per1H - per2H;
				height *= 1.0;						//hh = 0.5 + (hh - 0.5) * WaveDamp;
				return float4(height, per1H, 0, 1);
			}
			ENDCG
		}
	}
}
