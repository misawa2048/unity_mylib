Shader "Custom/CutOutByUVR" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_DispUVR ("Start&EndRadius (UVR)", Vector)=(0,0,1,1)
		_Fade("fade", Range(1.0,10.0))=10.0
	}
	SubShader {
		Tags { "RenderType"="Transprent" "Queue"="Transparent"}
		LOD 200
		CULL Off
		
		CGPROGRAM
		#pragma surface surf Lambert alpha

		sampler2D _MainTex;
		half4 _Color;
		half4 _DispUVR;
		half _Fade;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			half2 tmps = (_DispUVR.xy*0.5-length(IN.uv_MainTex - 0.5))*_Fade*_Fade;
			half2 tmpe = (_DispUVR.zw*0.5-length(IN.uv_MainTex - 0.5))*_Fade*_Fade;
			half2 tmp = saturate(tmpe)-saturate(tmps);
			o.Emission = c.rgb*_Color.rgb;
			o.Alpha = c.a*_Color.a*tmp.x;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
