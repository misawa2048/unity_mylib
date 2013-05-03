Shader "Custom/Textured Colored" {
Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_CmpR ("CmpColorR", Color) = (0.5,0,0,1)
		_CmpG ("CmpColorG", Color) = (0.1,0,0.4,1)
		_CmpB ("CmpColorB", Color) = (0,0,0.5,1)
		_Pow("Power", Range(1,64))=16
//		_Contrast("Contrast", Range(-0.1,2.0))=0.0		
}
SubShader {
    Pass {

CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members ipow,ictr)
#pragma exclude_renderers d3d11 xbox360
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

sampler2D _MainTex;
float4 _CmpR;
float4 _CmpG;
float4 _CmpB;
float _Pow;
//float _Contrast;

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
	half4 outCol;
	half4 nc = normalize(tex2D (_MainTex, i.uv)+0.001);
	half ct;
	ct = pow(dot(normalize(_CmpR+0.001),nc),_Pow);
//	ct = (ct-(cos(ct*3.14159265358979323846264)*2+1)*_Contrast)*0.5;
	outCol = (_CmpR*ct);

	ct = pow(dot(normalize(_CmpG+0.001),nc),_Pow);
//	ct = (ct-(cos(ct*3.14159265358979323846264)*2+1)*_Contrast)*0.5;
	outCol += (_CmpG*ct);
			
	ct = pow(dot(normalize(_CmpB+0.001),nc),_Pow);
//	ct = (ct-(cos(ct*3.14159265358979323846264)*2+1)*_Contrast)*0.5;
	outCol += (_CmpB*ct);
			
    return outCol;
}

half getContrast(half4 incol, half4 nc) : COLOR
{
	half4 outCol;
	half ct = pow(dot(normalize(incol+0.001),nc),_Pow);
//	ct = (ct-(cos(ct*3.14159265358979323846264)*2+1)*_Contrast)*0.5;
	outCol = (incol*ct);
	return outCol;
}

ENDCG

    }
}
Fallback "VertexLit"
} 
