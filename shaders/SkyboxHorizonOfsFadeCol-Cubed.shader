Shader "Skybox/CubemapHorizonOfsFadeCol" {
Properties {
	_Tint ("Tint Color", Color) = (.5, .5, .5, .5)
	[Gamma] _Exposure ("Exposure", Range(0, 8)) = 1.0
	_Rotation ("Rotation", Range(0, 360)) = 0
	_HeightOfs ("Height Offset", Range(-1, 1)) = 0
	_FadeRate("Fade Rate", Range(0, 1)) = 0
	_Color ("Tex1 Color", Color) = (1,1,1,1)
	_Color2 ("Tex2 Color", Color) = (1,1,1,1)
	[NoScaleOffset] _Tex ("Cubemap   (HDR)", Cube) = "grey" {}
	[NoScaleOffset] _Tex2("Cubemap2  (HDR)", Cube) = "grey" {}
}

SubShader {
	Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
	Cull Off ZWrite Off

	Pass {
		
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma target 2.0

		#include "UnityCG.cginc"

		#pragma multi_compile _TEX_BOTH _TEX_TEX1 _TEX_TEX2


		#define SKYBOX_TEX_BOTH 0
		#define SKYBOX_TEX_TEX1 1
		#define SKYBOX_TEX_TEX2 2

		#if defined(_TEX_TEX1)
			#define SKYBOX_TEX SKYBOX_TEX_TEX1
		#elif defined(_TEX_TEX2)
			#define SKYBOX_TEX SKYBOX_TEX_TEX2
		#else
			#define SKYBOX_TEX SKYBOX_TEX_BOTH
		#endif

		#if SKYBOX_TEX != SKYBOX_TEX_TEX2
		samplerCUBE _Tex;
		uniform half4 _Color;
		half4 _Tex_HDR;
		#endif
		#if SKYBOX_TEX != SKYBOX_TEX_TEX1
		samplerCUBE _Tex2;
		uniform half4 _Color2;
		half4 _Tex2_HDR;
		#endif
		half4 _Tint;
		half _Exposure;
		float _Rotation;
		uniform half _HeightOfs;
		uniform half _FadeRate;

		float3 RotateAroundYInDegrees (float3 vertex, float degrees)
		{
			float alpha = degrees * UNITY_PI / 180.0;
			float sina, cosa;
			sincos(alpha, sina, cosa);
			float2x2 m = float2x2(cosa, -sina, sina, cosa);
			return float3(mul(m, vertex.xz), vertex.y).xzy;
		}
		
		struct appdata_t {
			float4 vertex : POSITION;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		struct v2f {
			float4 vertex : SV_POSITION;
			float3 texcoord : TEXCOORD0;
			UNITY_VERTEX_OUTPUT_STEREO
		};

		v2f vert (appdata_t v)
		{
			v2f o;
			UNITY_SETUP_INSTANCE_ID(v);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
			float3 rotated = RotateAroundYInDegrees(v.vertex, _Rotation);
			o.vertex = UnityObjectToClipPos(rotated);
			o.texcoord = v.vertex.xyz+float3(0,_HeightOfs,0);
			return o;
		}

		fixed4 frag (v2f i) : SV_Target
		{
		#if SKYBOX_TEX == SKYBOX_TEX_TEX1
			half4 tex = texCUBE (_Tex, i.texcoord);
			half3 c = DecodeHDR (tex, _Tex_HDR) * _Color;
		#elif SKYBOX_TEX == SKYBOX_TEX_TEX2
			half4 tex = texCUBE (_Tex2, i.texcoord);
			half3 c = DecodeHDR (tex, _Tex2_HDR) * _Color2;
		#else
			half4 tex = texCUBE (_Tex, i.texcoord);
			half4 tex2 = texCUBE(_Tex2, i.texcoord);
			half3 c = DecodeHDR (tex, _Tex_HDR) * _Color * (1- _FadeRate) + DecodeHDR(tex2, _Tex2_HDR) * _Color2 * _FadeRate;
		#endif

			c = c * _Tint.rgb * unity_ColorSpaceDouble.rgb * _Exposure;
			return half4(c, 1);
		}
		ENDCG 
	}
} 	


Fallback Off

}
