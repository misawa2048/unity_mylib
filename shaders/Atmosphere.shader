Shader "Transparent/Atmosphere" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags {  "RenderType" = "Transparent" "Queue" = "Transparent" }
		LOD 200
		ZWrite  Off
		Cull Off
		Lighting Off
		SeparateSpecular Off
		
		CGPROGRAM
		#pragma surface surf Lambert alpha

		sampler2D _MainTex;
		float4 _Color;

		struct Input {
 			float2 uv_MainTex;
 			float3 viewDir;
 			float3 worldNormal;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			half NdotL = dot (IN.worldNormal, IN.viewDir);
			if(NdotL>0){ NdotL*=-0.2; }
			o.Emission = c.rgb*_Color.rgb;
			o.Alpha = c.a*_Color.a*-NdotL;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
