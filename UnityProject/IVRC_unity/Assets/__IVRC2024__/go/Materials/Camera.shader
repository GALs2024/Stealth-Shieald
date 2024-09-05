Shader "Custom/Grayscale"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
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
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);
                // ÉÇÉmÉNÉçïœä∑: RGBÇìØÇ∂ílÇ…Ç∑ÇÈ
                float gray = dot(col.rgb, float3(0.299, 0.587, 0.114)); // ÉÇÉmÉNÉçÇ…Ç∑ÇÈåˆéÆ
                return half4(gray, gray, gray, col.a);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
