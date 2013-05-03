Shader "Custom/ColoredDecal1TexVtxCol" {
	Properties {
		_Color ("Master Color", Color) = (1,1,1,1)
		_MainCol ("Main Color", Color) = (1,1,1,1)
		_DecalCol ("Decal Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_DecalOfs ("Decal Offset", Vector) = (0,0,0,0)
	}

SubShader {
		Tags { "RenderType"="Opaque" "Queue"="Geometry" "IgnoreProjector"="true" }
	Pass {
//		AlphaTest Greater 0.5
		ZTest Less
		ZWrite On
//		ColorMask RGBA
		Blend SrcAlpha OneMinusSrcAlpha

		
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"
		
		sampler2D _MainTex;
		half4 _Color;
		half4 _MainCol;
		half4 _DecalCol;
		half4 _DecalOfs;
		
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
			half4 tm0 = tex2D (_MainTex, i.uv) * _MainCol;
			half4 td0 = tex2D (_MainTex, i.uv + _DecalOfs) * _DecalCol;
			half3 emi = (tm0.rgb * (tm0.a * (1-td0.a)) + td0.rgb * td0.a) * _Color.rgb;
			half alp = tm0.a * _Color.a;
		
			half4 outCol = half4(emi.r,emi.g,emi.b,alp); //tex2D (_MainTex, i.uv);
			outCol *= i.color;
			clip(outCol.a - 0.1);
			return outCol;
		}
		ENDCG
	} 
	}
	FallBack "Unlit"
}
