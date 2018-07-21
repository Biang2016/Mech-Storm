Shader "ShaderTest" 
{
 Properties {
    _Color ("Color Tint", Color) = (1.0, 1.0, 1.0, 1.0)
 }
 SubShader
 {
     Pass{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag

        fixed4 _Color;

        struct a2v{
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float4 textcoord : TEXCOORD0;
            float4 color : COLOR;
        };

        struct v2f{
            float4 pos : SV_POSITION;
            fixed3 color : COLOR0;
        };

        void vert(in a2v v, out v2f o)
        {
            o.pos = UnityObjectToClipPos(v.vertex);
            o.color = v.color;
        }

        fixed4 frag(v2f i):SV_Target
        {
            fixed3 c = i.color;
            // c *= _Color.rgb;
            return fixed4(i.color, 1.0);
        }
        ENDCG
     }
 }

 
}