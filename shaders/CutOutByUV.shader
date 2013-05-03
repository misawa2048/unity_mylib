Shader "Custom/CutOutByUV" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_DispUV ("Start&EndPoint (UV)", Vector)=(0,0,1,1)
		_Fade("fade", Range(1.0,10.0))=10.0
	}
	SubShader {
		Tags { "RenderType"="Transprent" "Queue"="Transparent"}
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert alpha

		sampler2D _MainTex;
		half4 _DispUV;
		half _Fade;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			half sqLen=0;
			half tmp;
			tmp = IN.uv_MainTex.x - _DispUV.x;
			sqLen += ( tmp < 0 ? tmp*tmp : 0);
			tmp = IN.uv_MainTex.x - _DispUV.z;
			sqLen += ( tmp > 0 ? tmp*tmp : 0);
			tmp = IN.uv_MainTex.y - _DispUV.y;
			sqLen += ( tmp < 0 ? tmp*tmp : 0);
			tmp = IN.uv_MainTex.y - _DispUV.w;
			sqLen += ( tmp > 0 ? tmp*tmp : 0);

			o.Emission = c.rgb;
			o.Alpha = (1-sqrt(sqLen)*_Fade*_Fade);
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
