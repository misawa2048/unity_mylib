Shader "Transparent/Atmosphere" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_CloudRate ("Cloud rate", float) = 0.1
	}
	SubShader {
		Tags {  "RenderType" = "Transparent" "Queue" = "Transparent" }
		LOD 200
		ZWrite  Off
		Cull Off
		ColorMaterial Emission
		Lighting Off
		SeparateSpecular Off
		
		CGPROGRAM
		#pragma surface surf Lambert alpha

		sampler2D _MainTex;
		float4 _Color;
		half _CloudRate;

		struct Input {
 			float2 uv_MainTex;
 			float3 viewDir;
 			float3 worldNormal;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 t = tex2D (_MainTex, IN.uv_MainTex);
			half NdotL = dot (IN.worldNormal, IN.viewDir);
			half4 c = _Color;
			c.a*=-NdotL;
			if(NdotL>0){ c.a=t.rgb*NdotL*_CloudRate; }
			o.Emission = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
