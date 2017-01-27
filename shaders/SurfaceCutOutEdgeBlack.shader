Shader "Custom/SurfaceCutOffEdgeBlackShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_AlphaTex ("Texture", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _AlphaTex;

		struct Input {
			float2 uv_MainTex;
			float2 uv_AlphaTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		half _Cutoff;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			half alpha = tex2D(_AlphaTex, IN.uv_AlphaTex).a;
			clip (alpha - _Cutoff);
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb * min(1,alpha - _Cutoff + (alpha - _Cutoff)/max(_Cutoff,0.001));
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
//			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
