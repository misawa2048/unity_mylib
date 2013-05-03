Shader "GUI/EdgeOutlineByAlpha" {
	Properties {
		_Color ("MainColor", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "clear" {}
		_FontTex ("Font (RGB)", 2D) = "white" {}
		_Offset ("Check Offset", Float)=0.01
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert alpha

		sampler2D _MainTex;
		sampler2D _FontTex;
		half4 _Color;
		half _Offset;

		struct Input {
			float2 uv_MainTex;
			float2 uv_FontTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half a = tex2D (_FontTex, IN.uv_FontTex).a;
			half ae=0;
			ae += tex2D(_FontTex, IN.uv_FontTex+float2(_Offset,_Offset)).a;
			ae += tex2D(_FontTex, IN.uv_FontTex+float2(-_Offset,_Offset)).a;
			ae += tex2D(_FontTex, IN.uv_FontTex+float2(_Offset,-_Offset)).a;
			ae += tex2D(_FontTex, IN.uv_FontTex+float2(-_Offset,-_Offset)).a;
			ae += tex2D(_FontTex, IN.uv_FontTex+float2(_Offset,0)).a;
			ae += tex2D(_FontTex, IN.uv_FontTex+float2(-_Offset,0)).a;
			ae += tex2D(_FontTex, IN.uv_FontTex+float2(0,_Offset)).a;
			ae += tex2D(_FontTex, IN.uv_FontTex+float2(0,-_Offset)).a;
			ae *= 0.125;
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Emission = c*ae+_Color.rgb*(1-ae)*8;
			o.Alpha = c.a*ae+a*_Color.a*(1-ae)*8;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
