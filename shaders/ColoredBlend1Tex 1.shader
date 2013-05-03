Shader "Custom/ColoredBlend1Tex" {
	Properties {
		_Color ("Master Color", Color) = (0.5,0.5,0.5,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Offset ("Offset (0&1)", Vector) = (0,0,0,0)
		_BlendRate ("Blend Rate", Range(0,1)) = 0
	}
	SubShader {
		Tags { "RenderType" = "Opaque" "Queue" = "Transparent" }
//		LOD 200
//		ZTest LEqual
//		ZWrite On
		
		CGPROGRAM
		#pragma surface surf Lambert alpha noambient novertexlights nolightmap noforwardadd 

		float4 _Color;
		sampler2D _MainTex;
		float4 _Offset;
		float _BlendRate;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 tm0 = tex2D (_MainTex, IN.uv_MainTex + _Offset.xy);
			half4 td0 = tex2D (_MainTex, IN.uv_MainTex + _Offset.zw);
			float4 col = (tm0 * tm0.a * (1-_BlendRate) + td0 * td0.a * _BlendRate);
			col.a = (tm0.a * (1-_BlendRate) + td0.a * _BlendRate) * _Color.a;
			o.Emission = col.rgb + ((_Color.rgb-0.5)*2.0);
			o.Alpha = col.a * _Color.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
