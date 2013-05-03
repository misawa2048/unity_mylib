Shader "Custom/SpriteVtxColEdgeAlpha" {
	Properties {
		_Color ("Master Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_CutoutMin ("CutoutMin", Range(0.0,1.0)) = 0.5
		_CutoutMax ("CutoutMax", Range(0.0,1.0)) = 0.5
	}

SubShader {
	Tags { "RenderType"="Opaque" "Queue"="Geometry" "IgnoreProjector"="true" }
	Pass {
		ZTest Less
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"
		
		sampler2D _MainTex;
		half4 _Color;
		half _CutoutMin;
		half _CutoutMax;
		
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
			half4 emi = tex2D (_MainTex, i.uv) * _Color;
			clip(emi.a - _CutoutMin);
			clip(_CutoutMax - emi.a);
			emi.rgb = emi.rgb*2+max(half3(0,0,0),_Color.rgb-0.5)*2;
			emi *= i.color;
			emi.rgb = emi.rgb*2+max(half3(0,0,0),i.color.rgb-0.5)*2;
			return emi;
		}
		ENDCG
	} 
	}
	FallBack "Unlit"
}
