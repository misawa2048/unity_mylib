Shader "Unlit/BumpedProjector"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_BumpMap("Normal Map", 2D) = "bump" {}
		[HDR] _Color("Tint", Color) = (1,1,1,1)
		_Intensity("Intensity Offset", Float) = 1
		_BumpIntensity("Bump Intensity Offset", Float) = 1
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
		float2 uv : TEXCOORD0;
		UNITY_FOG_COORDS(1)
		float4 vertex : SV_POSITION;
		float3 normal : NORMAL;
	};

	sampler2D _MainTex;
	float4 _MainTex_ST;
	float4 _Color;
	float _Intensity;
	sampler2D _BumpMap;
	float _BumpIntensity;

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		UNITY_TRANSFER_FOG(o,o.vertex);
		o.normal = v.normal;
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		// sample the texture
		half3 tnormal = UnpackNormal(tex2D(_BumpMap, i.uv));
		half3 lnormal = normalize(half3(0.5, -0.9, 0.5));
		half ncolor = dot(lnormal, tnormal)*_BumpIntensity;
		half4 col = 1;
		col.rgb = tex2D(_MainTex, i.uv) * _Color+ ncolor + _Intensity;
	// apply fog
	//UNITY_APPLY_FOG(i.fogCoord, col);
	return col;
	}
		ENDCG
	}
	}
}
/*
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// https://docs.unity3d.com/ja/2018.4/Manual/SL-VertexFragmentShaderExamples.html
Shader "Unlit/ProjectorBump"
{
Properties{
_MainTex("Base texture", 2D) = "white" {}
_BumpMap("Normal Map", 2D) = "bump" {}
[HDR] _Color("Tint", Color) = (1,1,1,1)
_Intensity("Intensity Offset", Float) = 1
}
SubShader
{
Pass
{
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

// 前のシェーダーと完全に同じです
struct v2f {
float3 worldPos : TEXCOORD0;
half3 tspace0 : TEXCOORD1;
half3 tspace1 : TEXCOORD2;
half3 tspace2 : TEXCOORD3;
float2 uv : TEXCOORD4;
float4 pos : SV_POSITION;
};
v2f vert(float4 vertex : POSITION, float3 normal : NORMAL, float4 tangent : TANGENT, float2 uv : TEXCOORD0)
{
v2f o;
o.pos = UnityObjectToClipPos(vertex);
o.worldPos = mul(unity_ObjectToWorld, vertex).xyz;
half3 wNormal = UnityObjectToWorldNormal(normal);
half3 wTangent = UnityObjectToWorldDir(tangent.xyz);
half tangentSign = tangent.w * unity_WorldTransformParams.w;
half3 wBitangent = cross(wNormal, wTangent) * tangentSign;
o.tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
o.tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
o.tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);
o.uv = uv;
return o;
}

// シェーダープロパティーのテクスチャ
sampler2D _MainTex;
sampler2D _OcclusionMap;
sampler2D _BumpMap;

fixed4 frag(v2f i) : SV_Target
{
half3 tnormal = UnpackNormal(tex2D(_BumpMap, i.uv));
fixed ncolor = pow(dot(half3(0, 0, 1), tnormal),10);
fixed4 c = 0;
c.rgb = tnormal;
fixed3 baseColor = ncolor; // tex2D(_MainTex, i.uv).rgb;
c.rgb = baseColor;

return c;
}
ENDCG
}
}
}
*/


