Shader "Custom/TransparentSelfIluminVtxCol" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Color", Color) = (0.5,0,0,1)
	}

SubShader {
	Pass {
		Tags { "RenderType"="Opaque" "Queue"="Geometry" "IgnoreProjector"="False" }
		Blend SrcAlpha OneMinusSrcAlpha
//		AlphaTest GEqual 0.5
		ZTest Less
		ZWrite Off
		ColorMask RGBA

		
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"
		
		sampler2D _MainTex;
		half4 _Color;
		
		struct v2f {
		    float4  pos : SV_POSITION;
		    float2  uv : TEXCOORD0;
		    float4 color : COLOR;
		};
		
		float4 _MainTex_ST;
		
		v2f vert (appdata_full v)
		{
		    v2f o;
		    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
		    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
		    o.color = v.color;
		    return o;
		}
		
		half4 frag (v2f i) : COLOR
		{
			half4 outCol = tex2D (_MainTex, i.uv);
			outCol *= i.color;
//			clip(outCol.a - 0.1);
			return outCol;
		}
		ENDCG
	} 
	}
	FallBack "Unlit"
}
