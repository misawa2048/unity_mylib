Shader "Unlit/Projector"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		[HDR] _Color("Tint", Color) = (1,1,1,1)
		_Intensity("Intensity Offset", Float) = 0
		_FogIntensity("Fog Intensity", Float) = 1
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
			float4 _Color;
			float _Intensity;
			float _FogIntensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) * _Color + _Intensity;
                // apply fog
				fixed4 fogCol = col;
                UNITY_APPLY_FOG(i.fogCoord, fogCol);
				col.rgb = (1-fogCol)*(1- _FogIntensity)*col+fogCol;
                return col;
            }
            ENDCG
        }
    }
}
