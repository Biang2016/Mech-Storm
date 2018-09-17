// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/LakeTile"
{
	Properties
	{
		_NormalSample ("Normal", 2D) = "white" {}
		_FresnelSample ("Fresnel", 2D) = "black" {}
		_MaskSample ("Mask", 2D) = "red" {}
		_PatternSample ("Pattern", 2D) = "black" {}
		_DriftSample ("Drift", 2D) = "black" {}
		_LandSample ("Land", 2D) = "black" {}

		_AnimationSpeed("AnimationSpeed", Float) = 1
		_Tiling("Tiling", Float) = 1
		_NormalIntensity("NormalIntensity", Float) = 1
		_Wave1("Wave1 (Vx, Vy, Tiling, Offset)", Vector) = (0.3, 0.009, 1, 0)
		_Wave2("Wave2 (Vx, Vy, Tiling, Offset)", Vector) = (-0.19, -0.0013, 1.2, 0.37)
		_DriftWave("DriftWave (Vx, Vy, Tiling, Speed)", Vector) = (1, 1, 1, 0.1)
		_Light("Light", Vector) = (0, 0, 1, 0)
		_BaseColor("BaseColor", Color) = (0, 0, 1, 1)
		_TransitionBaseColor("TransitionBaseColor", Color) = (0, 0, 1, 1)
		_ShallowColor("ShallowColor", Color) = (0.5, 0.7, 1, 1)
		_TransitionShallowColor("TransitionShallowColor", Color) = (0.5, 0.7, 1, 1)
		_SpecularColor("SpecularColor", Color) = (1, 1, 1, 1)
		_SpecularIntensity("SpecularIntensity", Float) = 1
		_SpecularShiness("SpecularShiness", Float) = 80
		_SkyLuminance("SkyLuminance", Float) = 1.5
		_SkyNormal("SkyNormal", Float) = 3
		_DriftOpacity("DriftOpacity", Float) = 1
		_DriftDisturb("DriftDisturb", Float) = 0
		_LandTiling("LandTiling", Float) = 1
	}
	SubShader
	{
		Tags {  "QUEUE"="Background+20" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
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
				float2 uv : TEXCOORD1;
				float2 uv2 : TEXCOORD2;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float3 position : TEXCOORD2;
				float3 viewDirection : TEXCOORD3;
				float2 screenPos : TEXCOORD4;
				

				//UNITY_FOG_COORDS(3)
			};

			sampler2D _NormalSample;
			sampler2D _FresnelSample;
			sampler2D _MaskSample;
			sampler2D _PatternSample;
			sampler2D _DriftSample;
			sampler2D _LandSample;

			float _AnimationSpeed;
			float _Tiling;
			float _NormalIntensity;
			float4 _Wave1;
			float4 _Wave2;
			float4 _DriftWave;
			float3 _Light;
			float4 _BaseColor;
			float4 _TransitionBaseColor;
			float4 _ShallowColor;
			float4 _TransitionShallowColor;
			float4 _SpecularColor;
			float _SpecularIntensity;
			float _SpecularShiness;
			float _SkyLuminance;
			float _SkyNormal;
			float _DriftOpacity;
			float _DriftDisturb;
			float _LandTiling;

			float _GolbalNight;

			v2f vert (appdata v)
			{
				v2f o;

				o.vertex = UnityObjectToClipPos(v.vertex);

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

				o.uv = v.uv;
				o.uv2 = v.uv2;

				o.position = worldPos;

				o.viewDirection = -mul(UNITY_MATRIX_MV, v.vertex).xyz;

				o.screenPos = ComputeScreenPos(o.vertex / o.vertex.w);

				UNITY_TRANSFER_FOG(o,o.vertex);

				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float2 uv1 = (i.position.xy * _Wave1.z + _Wave1.w + _Wave1.xy * _AnimationSpeed * _Time.x) * _Tiling;
				float2 uv2 = (i.position.xy * _Wave2.z + _Wave2.w + _Wave2.xy * _AnimationSpeed * _Time.x) * _Tiling;

				float3 normal1 = tex2D(_NormalSample, uv1).rgb - float3(0.5, 0.5, 0);
				float3 normal2 = tex2D(_NormalSample, uv2).rgb - float3(0.5, 0.5, 0);

				normal1.xy *= 2 * _NormalIntensity;
				normal2.xy *= 2 * _NormalIntensity;

				float3 normal = normalize(normal1 + normal2);
				float3 viewDir = normalize(i.viewDirection);

				float specular = dot(normal, normalize(normalize(_Light) + viewDir));

				fixed4 col = lerp(_TransitionBaseColor, _BaseColor, _GolbalNight);

				float2 uv_d = i.position.xy * _DriftWave.z + _DriftWave.xy * (_DriftWave.w * _AnimationSpeed * _Time.x) + normal.xy * _DriftWave.z * _DriftDisturb;

				fixed4 pattern = tex2D(_PatternSample, i.uv);
				fixed4 drift = tex2D(_DriftSample, uv_d);
				drift.rgb = drift.rgb * drift.a + lerp(_TransitionShallowColor, _ShallowColor, _GolbalNight) * (1 - drift.a);

				float4 mask = tex2D(_MaskSample, i.uv2);

				float driftO = mask.g * _DriftOpacity;
				col.rgb = col.rgb * (1 - driftO) + drift.rgb * driftO;
				//col.rgb = drift.rgb;

				float3 specularColor = clamp(_SpecularColor.rgb * pow(specular, _SpecularShiness) * _SpecularIntensity * _GolbalNight, 0, 1);

				col.rgb += specularColor;

				col.rgb += tex2D(_FresnelSample, i.screenPos.xy).rgb * _SkyLuminance * abs(normal.y + _SkyNormal / (_SkyNormal + 1));

				float3 land = tex2D(_LandSample, i.position.xy * _LandTiling);

				//col.rgb = col.rgb * mask.r * (1 - pattern.a) + pattern.rgb * pattern.a * mask.r + land * (1 - mask.r);
				col.rgb = col.rgb * mask.r * (1 - pattern.a) + pattern.rgb * pattern.a * mask.r;
				//col.rgb *= _GolbalNight;

				col.a = mask.r;
				
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);

				return col;
			}
			ENDCG
		}
	}
}
