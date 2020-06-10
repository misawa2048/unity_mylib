Shader "ModelToBump/NormalDispShaderBothSide"
{
    Properties
    {
        _BumpMap ("TextureNormal Map", 2D) = "bump" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
		CULL Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
				float3 tangent : TANGENT;
            };

            struct v2f
            {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float3 tangent : TANGENT;
			};

			float4x4 InvTangentMatrix(
				float3 tangent,
				float3 binormal,
				float3 normal)
			{
				float4x4 mat = float4x4(float4(tangent.x, tangent.y, tangent.z, 0.0f),
					float4(binormal.x, binormal.y, binormal.z, 0.0f),
					float4(normal.x, normal.y, normal.z, 0.0f),
					float4(0, 0, 0, 1)
					);
				return transpose(mat);
			}

            sampler2D _BumpMap;
            float4 _BumpMap_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				float3 nor = normalize(v.normal);
				float3 tan = normalize(v.tangent);
				float3 binor = cross(nor, tan);
				float3 wNormal = UnityObjectToWorldNormal(v.normal);
				wNormal.xy *= -1;
				o.normal = wNormal;
				o.tangent = UnityObjectToWorldNormal(v.tangent);

				return o;
			}

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
				float3 bumpNormal = UnpackNormal(tex2D(_BumpMap, i.uv));
				float3 normal = (i.normal.z > 0.05) ? i.normal : -i.normal;
				normal = normalize(normal + mul(bumpNormal , i.tangent)*0.5);
				normal = (1 + normal)*0.5;
				fixed4 col = fixed4(normal,1);
                return col;
            }
            ENDCG
        }
    }
}
