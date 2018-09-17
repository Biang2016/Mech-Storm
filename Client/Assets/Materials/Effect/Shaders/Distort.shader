Shader "Dvornik/Distort" {
Properties {
	_Refraction ("Refraction", Range (0.00, 100.0)) = 1.0
	_SceneTexture ("Scene (RGBA)", 2D) = "white" {}
	_DistortTex ("Base (RGB)", 2D) = "white" {}
}

SubShader 
{	
	//Tags { "RenderType"="Transparent" "Queue"="Overlay" }
	Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
	LOD 100
	ZTest Off
	ZWrite Off
	GrabPass 
	{ 
		
	}
	
CGPROGRAM
#pragma exclude_renderers gles

#pragma surface surf NoLighting
#pragma vertex vert

fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
    {
        fixed4 c;
        c.rgb = s.Albedo; 
        c.a = s.Alpha;
        return c;
    }

//sampler2D _GrabTexture : register(s0);
sampler2D _SceneTexture : register(s1);
sampler2D _DistortTex : register(s2);
float _Refraction;

//float4 _GrabTexture_TexelSize;
float4 _SceneTexture_TexelSize;

struct Input {
	float2 uv_DistortTex;
	float3 color;
	float3 worldRefl; 
	float4 screenPos;
	INTERNAL_DATA
};

void vert (inout appdata_full v, out Input o) {
  UNITY_INITIALIZE_OUTPUT(Input,o);
  o.color = v.color;
}

void surf (Input IN, inout SurfaceOutput o) 
{
    float3 distort = tex2D(_DistortTex, IN.uv_DistortTex) * IN.color.rgb;
    float2 offset = distort * _Refraction * _SceneTexture_TexelSize.xy;
	IN.screenPos.xy = offset * IN.screenPos.z + IN.screenPos.xy;
	float4 refrColor = tex2Dproj(_SceneTexture, IN.screenPos);
	o.Alpha = refrColor.a;
	o.Emission = refrColor.rgb;
}
ENDCG
}
FallBack "Diffuse"
}