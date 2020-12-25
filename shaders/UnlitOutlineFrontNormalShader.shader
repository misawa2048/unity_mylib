Shader "Unlit/UnlitOutlineFrontNormalShader"
{
    Properties
    {
		_Color("Outline color", Color) = (0,0,0,1)
		_Gap("Nomal gap", Range(0, 1)) = 0.005
		_ZOfs("Z offset", Range(0, 0.1)) = 0.02
	}
    SubShader
    {
		Tags { "RenderType"="Opaque" "Queue"="Geometry+10" }
        LOD 100
		CULL Front
		Offset 10,10 // 

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
				float3 normal : NORMAL;
	};

            struct v2f
            {
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

			float4 _Color;
			float _Gap;
			float _ZOfs;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex+ v.normal*_Gap);
				o.vertex.z -= _ZOfs;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = _Color;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
