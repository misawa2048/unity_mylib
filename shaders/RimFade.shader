Shader "FX/RimFade" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_RimGain ("Rim Gain", Range(0.1,5.0)) = 1.0
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

		struct Input {
 			float2 uv_MainTex;
 			float3 viewDir;
 			float3 worldNormal;
		};

		sampler2D _MainTex;
		float4 _Color;
		float _RimGain;

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			half np = pow((abs(dot(IN.worldNormal, normalize(IN.viewDir)))),_RimGain);
			o.Emission  = _Color.rgb*c.rgb;
			o.Alpha = _Color.a*c.a*(np);
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
