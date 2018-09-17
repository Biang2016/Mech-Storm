// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/SurfaceWater"
{
	Properties
	{
		_NormalSample ("Normal", 2D) = "white" {}
		_FresnelSample ("Fresnel", 2D) = "black" {}
		_MaskSample ("Mask", 2D) = "red" {}
		_GroundSample ("Ground", 2D) = "black" {}
		_SkySample ("Sky", 2D) = "black" {}

		_AnimationSpeed("AnimationSpeed", Float) = 1
		_NormalTiling("NormalTiling", Float) = 1
		_GroundTiling("GroundTiling", Float) = 1
		_NormalIntensity("NormalIntensity", Float) = 1
		_RefractionIntensity("RefractionIntensity", Float) = 1
		_Wave1("Wave1 (Vx, Vy, Tiling, Offset)", Vector) = (0.3, 0.009, 1, 0)
		_Wave2("Wave2 (Vx, Vy, Tiling, Offset)", Vector) = (-0.19, -0.0013, 1.2, 0.37)
		_Light("Light", Vector) = (0, 0, 1, 0)
		_BaseColor("BaseColor", Color) = (0, 0, 1, 1)
		_SpecularColor("SpecularColor", Color) = (1, 1, 1, 1)
		_GroundLuminance("GroundLuminance", Float) = 1
		_SkyLuminance("SkyLuminance", Float) = 1
		_FresnelIntensity("FresnelIntensity", Float) = 1
		_SkyTiling("SkyTiling", Float) = 0.1
		_SkyOffset("SkyOffset (Ox, Oy, Vx, Vy)", Vector) = (0, 0, 0, 0)
		_Opacity("Opacity", Float) = 1
	}
	SubShader
	{
		Tags {  "QUEUE"="Background+8" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
		LOD 100

		Pass
		{
			ZWrite Off
			Blend One OneMinusSrcAlpha

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
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 position : TEXCOORD1;
				float3 viewDirection : TEXCOORD2;
				float2 screenPos : TEXCOORD3;

				UNITY_FOG_COORDS(3)
			};

			sampler2D _NormalSample;
			sampler2D _FresnelSample;
			sampler2D _MaskSample;
			sampler2D _LandSample;
			sampler2D _GroundSample;
			sampler2D _SkySample;

			float _AnimationSpeed;
			float _NormalTiling;
			float _GroundTiling;
			float _NormalIntensity;
			float _RefractionIntensity;
			float4 _Wave1;
			float4 _Wave2;
			float3 _Light;
			float4 _BaseColor;
			float4 _SpecularColor;
			float _GroundLuminance;
			float _SkyLuminance;
			float _FresnelIntensity;
			float _SkyTiling;
			float4 _SkyOffset;
			float _Opacity;

			v2f vert (appdata v)
			{
				v2f o;

				o.vertex = UnityObjectToClipPos(v.vertex);

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

				o.uv = v.uv;

				o.position = worldPos;

				o.viewDirection = -mul(UNITY_MATRIX_MV, v.vertex).xyz;

				o.screenPos = ComputeScreenPos(o.vertex / o.vertex.w);

				UNITY_TRANSFER_FOG(o,o.vertex);

				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float2 uv1 = (i.position.xy * _Wave1.z + _Wave1.w + _Wave1.xy * _AnimationSpeed * _Time.x) * _NormalTiling;
				float2 uv2 = (i.position.xy * _Wave2.z + _Wave2.w + _Wave2.xy * _AnimationSpeed * _Time.x) * _NormalTiling;

				float3 normal1 = tex2D(_NormalSample, uv1).rgb - float3(0.5, 0.5, 0);
				float3 normal2 = tex2D(_NormalSample, uv2).rgb - float3(0.5, 0.5, 0);

				normal1.xy *= 2 * _NormalIntensity;
				normal2.xy *= 2 * _NormalIntensity;

				float3 normal = normalize(normal1 + normal2);
				float3 viewDir = normalize(i.viewDirection);

				float viewOnNoraml = dot(normal, viewDir);
				float3 lightDir = normal * viewOnNoraml * 2 - viewDir;
				float3 sunDir = normalize(_Light);
				float3 sunU = normalize(cross(sunDir, float3(0, 1, 0)));
				float3 sunV = cross(sunDir, sunU);
				float3 lightOffset = lightDir - sunDir;
				float2 lightCoord = (float2(dot(lightOffset, sunU), dot(lightOffset, sunV)) / _SkyTiling) / 2.0 + 0.5;


				fixed4 col = _BaseColor;

				float4 mask = tex2D(_MaskSample, i.uv);

				lightCoord += _SkyOffset.xy + _SkyOffset.zw * _AnimationSpeed * _Time.x;
				float3 reflectionColor = tex2D(_SkySample, lightCoord) * _SpecularColor.rgb * _SkyLuminance;

				float3 refractionColor = tex2D(_GroundSample, (i.uv + normal.xy * _RefractionIntensity) * _GroundTiling) * _SpecularColor.rgb * _GroundLuminance;

				float fresnel = tex2D(_FresnelSample, i.screenPos.xy).x * _FresnelIntensity;

				col.rgb += fresnel * reflectionColor;
				col.rgb += (1 - fresnel) * refractionColor;

				col.rgb *= mask.r * _Opacity;
				col.a = mask.r * _Opacity;

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);

				return col;
			}
			ENDCG
		}
	}
}
