Shader "Custom/SelfIluminBackGround" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
	  Tags {  "RenderType" = "Background" "Queue" = "Background" }
		LOD 200
		ZWrite  Off
//		Cull Off
		Lighting Off
		
		CGPROGRAM
		#pragma surface surf Lambert

		struct Input {
			float2 uv_MainTex;
		};

		sampler2D _MainTex;
		float4 _Color;

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
//			o.Emission  = c.rgb*_Color.rgb;
			o.Emission = c.rgb+((_Color.rgb-0.5)*2.0);
//			o.Alpha = _Color.a*c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
