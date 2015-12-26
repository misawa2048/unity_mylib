Shader "Legacy Shaders/QuantizedNormal"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Tags{ "RenderType" = "Opaque" }
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

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uvp.xy = TRANSFORM_TEX(v.uv, _MainTex);
		float3 objViewDir = normalize(ObjSpaceLightDir(v.vertex)+ _CosTime.yzw);
		float3 qNormal = normalize(sin(floor(v.normal * 5))*120+ _CosTime.w);

		o.uvp.z = (dot(objViewDir, qNormal)+1)*0.5;
		o.uvp.w = pow(1 - o.uvp.z, 3);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		fixed4 col = tex2D(_MainTex, i.uvp.xy)*i.uvp.z;

	return col;
	}
		ENDCG
	}
	}
		FallBack "Mobile/Diffuse"
}
