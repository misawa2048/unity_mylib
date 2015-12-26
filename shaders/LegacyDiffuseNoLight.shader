Shader "Legacy Shaders/DiffuseNoLight"
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
				float2 uv : TEXCOORD0;
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 uvp : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uvp.xy = TRANSFORM_TEX(v.uv, _MainTex);
				float3 objViewDir = normalize(ObjSpaceViewDir(v.vertex));
//				float3 viewDir = normalize(WorldSpaceViewDir(v.vertex));
//				float3 viewDir = _WorldSpaceCameraPos.xyz - mul(_Object2World, v.vertex).xyz;
				o.uvp.z = dot(objViewDir, v.normal);
				o.uvp.w = pow(1-o.uvp.z, 3);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uvp.xy)*i.uvp.z + i.uvp.w*0.5;

				return col;
			}
			ENDCG
		}
	}
	FallBack "Mobile/Diffuse"
}
