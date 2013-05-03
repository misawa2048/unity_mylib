Shader "GUI/EdgeEmbossByAlpha" {
	Properties {
		_Color ("MainColor", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_FontTex ("Font (RGB)", 2D) = "white" {}
		_Offset ("Check Offset (UV)", Vector)=(0.01,0.01,0,0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert alpha

		sampler2D _MainTex;
		sampler2D _FontTex;
		half4 _Color;
		half4 _Offset;

		struct Input {
			float2 uv_MainTex;
			float2 uv_FontTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half a = tex2D (_FontTex, IN.uv_FontTex).a;
			half ae = tex2D (_FontTex, IN.uv_FontTex+_Offset).a;
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Emission = (a*2-ae)*c*_Color.rgb;
			o.Alpha = a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
