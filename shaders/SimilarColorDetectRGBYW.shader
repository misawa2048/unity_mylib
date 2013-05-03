Shader "Custom/SimilarColorDetectRGBYW" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_CmpR ("CmpColorR", Color) = (0.5,0,0,1)
		_CmpG ("CmpColorG", Color) = (0.1,0.4,0,1)
		_CmpB ("CmpColorB", Color) = (0,0,0.5,1)
		_CmpY ("CmpColorY", Color) = (1,1,0.1,1)
		_CmpW ("CmpColorW", Color) = (1,1,1,1)
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
		half4 _CmpR;
		half4 _CmpG;
		half4 _CmpB;
		half4 _CmpY;
		half4 _CmpW;
		half _Pow;
		half _Contrast;
		
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
		
		half4 frag (v2f i) : COLOR
		{
			half4 outCol=(0,0,0,0);
			half4 nc = normalize(tex2D (_MainTex, i.uv)+0.001);
			half ct;
			ct = pow(dot(normalize(_CmpR+0.001),nc),_Pow);
			outCol += (_CmpR*ct);

			ct = pow(dot(normalize(_CmpG+0.001),nc),_Pow);
			outCol += (_CmpG*ct);
			
			ct = pow(dot(normalize(_CmpB+0.001),nc),_Pow);
			outCol += (_CmpB*ct);

			ct = pow(dot(normalize(_CmpY+0.001),nc),_Pow);
			outCol += (_CmpY*ct);

			ct = pow(dot(normalize(_CmpW+0.001),nc),_Pow);
			outCol += (_CmpW*ct);

			ct = (1-(cos(3.14159265358979323846264)*2+1)*_Contrast)*0.5;
			outCol *= ct;
			
			return outCol;
		}
		ENDCG
	} 
	}
	FallBack "Unlit"
}
