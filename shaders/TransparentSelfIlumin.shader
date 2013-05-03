Shader "Custom/TransparentSelfIlumin" {
	Properties {
		  _Color ("Main Color", Color) = (1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
	  Tags {  "RenderType" = "Transparent" "Queue" = "Transparent" }
		LOD 200
		ZWrite  On
//		Cull Off
		Lighting Off
		SeparateSpecular Off

		
		CGPROGRAM
		#pragma surface surf Lambert alpha

		struct Input {
			float2 uv_MainTex;
		};

		sampler2D _MainTex;
		float4 _Color;

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Emission  = _Color.rgb*c.rgb;
			o.Alpha = _Color.a*c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
