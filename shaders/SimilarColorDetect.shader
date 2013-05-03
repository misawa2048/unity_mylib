Shader "Custom/SimilarColorDetect" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("MainColor", Color) = (1,1,1,1)
		_CmpColor ("CmpColor", Color) = (0.5,0,0,1)
		_Pow("Power", Range(1,64))=16
		_Contrast("Contrast", Range(-0.1,5.0))=0.0		
	}

SubShader {
	Pass {
		Tags { "RenderType"="Opaque" "Queue"="Geometry" }
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"
		
		sampler2D _MainTex;
		float4 _Color;
		float4 _CmpColor;
		float _Pow;
		float _Contrast;
		
		struct v2f {
		    float4  pos : SV_POSITION;
		    float2  uv : TEXCOORD0;
		};
		
		float4 _MainTex_ST;
		
		v2f vert (appdata_base v)
		{
		    v2f o;
		    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
		    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
		    return o;
		}
		
		float4 frag (v2f i) : COLOR
		{
			half4 outCol;
			float4 c = tex2D (_MainTex, i.uv);
			float d = dot(normalize(_CmpColor+0.001),normalize(c+0.001));
			float p = pow(d,_Pow);
			float ct = (p-(cos(p*3.14159265358979323846264)*2+1)*_Contrast)*0.5;
			outCol.rgb = (_Color*ct).rgb;
			outCol.a = ct;
			
			return outCol;
		}
		ENDCG
	} 
	}
	FallBack "Unlit"
}
