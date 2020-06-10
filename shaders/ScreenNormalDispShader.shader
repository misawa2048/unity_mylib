Shader "Unlit/ScreenNormalDispShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			sampler2D _CameraDepthNormalsTexture;
			float4 _CameraDepthNormalsTexture_TexelSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float4 cdn = tex2D(_CameraDepthNormalsTexture, i.uv);
				// サンプリングした値をDecodeViewNormalStereo()で変換
				float3 n = DecodeViewNormalStereo(cdn);
				float3 wnormal = (n + 1)*0.5;

				// sample the texture
				fixed4 col = fixed4(wnormal.rgb,1); // tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
