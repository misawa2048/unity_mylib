Shader "Custom/FakeSphere" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
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
		half _Fade;

		struct Input {
			float2 uv_MainTex;
			float3 worldNormal; INTERNAL_DATA 
			float3 worldRefl;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			half d = length(IN.uv_MainTex-0.5)*2;
			half4 nml = half4(IN.worldNormal,1);
			o.Albedo = c.rgb*_Color.rgb;
			o.Alpha = c.a*_Color.a*(d>1?0:1);
			half4 onml = half4((IN.uv_MainTex-0.5),0.1,0);
			onml = normalize(onml);
			o.Normal = dot(onml,IN.worldRefl);
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
