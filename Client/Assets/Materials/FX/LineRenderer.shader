Shader "AndrewBox/LineRenderer" {
Properties{
_MainTex("Base (RGB)", 2D) = "white" {}
}
SubShader{
Tags{ "RenderType" = "Transparent" "IgnoreProjector" = "True" "Queue" = "Transparent" }
LOD 200
Blend SrcAlpha OneMinusSrcAlpha
CGPROGRAM
#pragma surface surf NoLight vertex:vert alpha noforwardadd

//光照方程，名字为Lighting接#pragma suface后的光照方程名称 
//lightDir :顶点到光源的单位向量
//viewDir  :顶点到摄像机的单位向量   
//atten       :关照的衰减系数 
float4 LightingNoLight(SurfaceOutput s, float3 lightDir, half3 viewDir, half atten)
{
float4 c;
c.rgb = s.Albedo;
c.a = s.Alpha;
return c;
}

sampler2D _MainTex;
fixed4 _SelfCol;

struct Input
{
float2 uv_MainTex;
float4 vertColor;
};

void vert(inout appdata_full v, out Input o)
{
o.vertColor = v.color;
o.uv_MainTex = v.texcoord;
}

void surf(Input IN, inout SurfaceOutput o)
{
half4 c = tex2D(_MainTex, IN.uv_MainTex);
o.Alpha = c.a * IN.vertColor.a;
o.Albedo = IN.vertColor.rgb;
}


ENDCG
}
FallBack "Diffuse"
}