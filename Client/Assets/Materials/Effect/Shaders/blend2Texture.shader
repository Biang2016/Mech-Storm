// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/*Shader "golden/Blend2Texture"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_AlphaTex("Texture", 2D) = "white" {}
		_Color("Color", color) = (1, 1, 1, 1)
		_Rain("Rain", float) = 0;
	}
	SubShader
		{
			Tags{ "QUEUE" = "Background+3" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" }
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
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					o.uv2 = TRANSFORM_TEX(v.uv, _AlphaTex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					// sample the texture
					fixed4 col = tex2D(_MainTex, i.uv);
					col.a = tex2D(_AlphaTex, i.uv2).r;
					col *= _Color ;
					col.rgb = col.rgb;
					return col;
				}
					ENDCG
			}
		}
}*/

Shader "golden/Blend2Texture"
{
   Properties {
      _Color ("Diffuse Material Color", Color) = (1,1,1,1) 
	  _SpecColor("Specular Material Color", Color) = (0.3176470588235294, 0.3176470588235294, 0.3176470588235294, 1.0)
	  _Shininess("Shininess", Float) = 80.5
	  
		_MainTex ("Foreground", 2D) = "white" {} 
		_AlphaTex ("Background", 2D) = "white" {}
		//_ForegroundNormal ("Foreground normal", 2D) = "bump" {}

		_Tiling("Tiling", Float) = 1    
   }

   CGINCLUDE // common code for all passes of all subshaders

      #include "UnityCG.cginc"
      uniform float4 _LightColor0; 
      uniform sampler2D _RainNormal; 
	  uniform sampler2D _MainTex;
	  uniform sampler2D _AlphaTex;
	  
      uniform float4 _BaseMap_ST;
      uniform float4 _BumpMap_ST;
	  float4 _MainTex_ST;
	  float4 _AlphaTex_ST;
      uniform float4 _Color; 
      uniform float4 _SpecColor; 
      uniform float _Shininess;
      uniform float _Tiling;
	  float _Rain;
	  float _GolbalRain;	
	  float _RainTilling;	

      struct vertexInput {
         float4 vertex : POSITION;
         float4 texcoord : TEXCOORD0;
         float3 normal : NORMAL;
         float4 tangent : TANGENT;
		 float2 uv2 : TEXCOORD1;
      };
      struct vertexOutput {
         float4 pos : SV_POSITION;
         float4 posWorld : TEXCOORD0;
         float2 tex : TEXCOORD1;
         float3 tangentWorld : TEXCOORD2;  
         float3 normalWorld : TEXCOORD3;
         float3 binormalWorld : TEXCOORD4;
		 float2 uv : TEXCOORD5;
		 float2 uv2 : TEXCOORD6;
      };

      vertexOutput vert(vertexInput input) 
      {
         vertexOutput output;

         float4x4 modelMatrix = unity_ObjectToWorld;
         float4x4 modelMatrixInverse = unity_WorldToObject; 

         output.tangentWorld = normalize(
            mul(modelMatrix, float4(input.tangent.xyz, 0.0)).xyz);
         output.normalWorld = normalize(
            mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
         output.binormalWorld = normalize(
            cross(output.normalWorld, output.tangentWorld) 
            * input.tangent.w); // tangent.w is specific to Unity

         output.posWorld = mul(modelMatrix, input.vertex);
         output.pos = UnityObjectToClipPos(input.vertex);
         output.tex = output.posWorld.xy * _Tiling;

		 output.uv = TRANSFORM_TEX(input.texcoord, _MainTex);
		 //output.uv2 = TRANSFORM_TEX(input.texcoord, _AlphaTex);
		 output.uv2 = input.uv2;

         return output;
      }
      
      float4 GetColor( vertexOutput input, sampler2D baseMap, sampler2D normalMap )
      {
		 float2 normal_tex = input.tex.xy * _RainTilling;
         float4 encodedNormal = tex2D(normalMap,normal_tex );

		 float4 baseColor =	tex2D(baseMap, input.uv);
		 baseColor.a = tex2D(_AlphaTex, input.uv2).r;    
		                         
         float3 localCoords = float3(2.0 * 0.5 - 1.0, 
             2.0 * (encodedNormal.g*0.2+0.5) - 1.0, 0.0);
         localCoords.z = sqrt(1.0 - dot(localCoords, localCoords));

         float3x3 local2WorldTranspose = float3x3(
            input.tangentWorld, 
            input.binormalWorld, 
            input.normalWorld);
            
         float3 normalDirection = 
            normalize(mul(localCoords, local2WorldTranspose));

         float3 viewDirection = normalize(
            _WorldSpaceCameraPos - input.posWorld.xyz);
            
         float3 lightDirection;
         float attenuation;

        lightDirection = normalize(_WorldSpaceLightPos0.xyz);

        float3 diffuseReflection = 
            baseColor * _LightColor0.rgb * max(0.0, dot(normalDirection, lightDirection));

         float3 specularReflection;
         if (dot(normalDirection, lightDirection) < 0.0) 
         {
            specularReflection = float3(0.0, 0.0, 0.0); 
         }
         else
         {
            //specularReflection = _LightColor0.rgb 
            //   * _SpecColor.rgb * pow(max(0.0, dot(
            //   reflect(-lightDirection, normalDirection), 
            //   viewDirection)), _Shininess);
				float3 SpecCol = float3(81/255.0, 81/255.0, 81/255.0);
			   specularReflection = _LightColor0.rgb 
               * SpecCol.rgb * pow(max(0.0, dot(
               reflect(-lightDirection, normalDirection), 
               viewDirection)), 80.5);
			   
         }
         float3 color = lerp( diffuseReflection + specularReflection, diffuseReflection, 1-baseColor.a );
		 if(_GolbalRain == 0)
			return baseColor;
		 else{
			 //if (_Rain == 0)
				 //return baseColor;//dot(normalDirection, lightDirection);
			 //else
			 //{
				if(_GolbalRain == 1)
					return float4(color, 1.0);//float4(color, 1.0);
				else
					return lerp(baseColor,float4(color, 1.0),1-_GolbalRain);
			 //}
		 }
      }
			
      float4 frag2(vertexOutput input) : COLOR
      {
      
		//float4 fore = GetColor( input, _MainTex, _RainNormal);
		float4 fore =	tex2D(_MainTex, input.uv);
		fixed4 col = fore;

		float2 uv2 = TRANSFORM_TEX(input.uv2, _AlphaTex);
		col.a = tex2D(_AlphaTex, input.uv2).a;	  
      	return col;
      }
      
   ENDCG


   SubShader {
         //Tags { "QUEUE"="Background+1" "LightMode" = "ForwardBase" "RenderType" = "Transparent"} 
		 Tags { "QUEUE" = "Background+3" "LightMode" = "ForwardBase" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent"} 
    
      Pass {      
            // pass for ambient light and first light source
		 ZWrite Off
		 Lighting On
		 Blend SrcAlpha OneMinusSrcAlpha
		 //Blend One one
         CGPROGRAM
		
            #pragma vertex vert  
            #pragma fragment frag2
            // the functions are defined in the CGINCLUDE part
         ENDCG
      }
 
   }
}