Shader "Custom/ColoredDecal1Tex" {
	Properties {
		_Color ("Master Color", Color) = (0.5,0.5,0.5,1)
		_MainCol ("Main Color", Color) = (0.5,0.5,0.5,1)
		_DecalCol ("Decal Color", Color) = (0.5,0.5,0.5,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_DecalOfs ("Decal (Scale&Offset)", Vector) = (1,1,0,0)
	}
	SubShader {
		Tags { "RenderType" = "Opaque" "Queue" = "Transparent" }
//		LOD 200
//		ZTest LEqual
//		ZWrite On
		
		CGPROGRAM
		#pragma surface surf Lambert alpha noambient novertexlights nolightmap noforwardadd 

		float4 _Color;
		float4 _MainCol;
		float4 _DecalCol;
		sampler2D _MainTex;
		float4 _DecalOfs;

		struct Input {
			float2 uv_MainTex;
			float2 uv_DecalTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 tm0c = tex2D (_MainTex, IN.uv_MainTex);
			half4 tm0 = (tm0c*tm0c.a + (_MainCol-0.5)*2.0);
			tm0.a = tm0c.a*_MainCol.a;
			half4 td0c = tex2D (_MainTex, IN.uv_MainTex*_DecalOfs.xy + _DecalOfs.zw);
			half4 td0 = (td0c*td0c.a + (_DecalCol-0.5)*2.0);
			td0.a = td0c.a*_DecalCol.a;
			o.Emission = (tm0.rgb * (tm0.a * (1-td0.a)) + td0.rgb * td0.a) + (_Color-0.5)*2.0;
			o.Alpha = _Color.a*clamp(tm0.a+td0.a,0,1);
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
