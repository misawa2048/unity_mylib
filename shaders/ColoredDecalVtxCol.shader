Shader "Custom/ColoredDecalVtxCol" {
	Properties {
		_Color ("Master Color", Color) = (1,1,1,1)
		_MainCol ("Main Color", Color) = (1,1,1,1)
		_DecalCol ("Decal Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_DecalTex ("Decal (RGB)", 2D) = "clear" {}
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
		sampler2D _DecalTex;
		half4 _Color;
		half4 _MainCol;
		half4 _DecalCol;
		
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
			half4 tm0 = tex2D (_MainTex, i.uv);
			half4 td0 = tex2D (_DecalTex, i.uv);
			tm0 = ((_MainCol<tm0) ? (2*tm0*_MainCol) : (_MainCol*2-1)*(1-tm0)+tm0)*tm0.a;
			td0 = ((_DecalCol<td0) ? (2*td0*_DecalCol) : (_DecalCol*2-1)*(1-td0)+td0)*td0.a;
			half4 emi = (tm0 * (1-td0.a) + td0 * td0.a);
			emi = (_Color<emi) ? (2*emi*_Color) : (_Color*2-1)*(1-emi)+emi;
			half alp = tm0.a * _Color.a;
		
			half4 outCol = half4(emi.r,emi.g,emi.b,alp); //tex2D (_MainTex, i.uv);
//			outCol *= i.color;
			clip(outCol.a - 0.01);
			return outCol;
		}
		ENDCG
	} 
	}
	FallBack "Unlit"
}
