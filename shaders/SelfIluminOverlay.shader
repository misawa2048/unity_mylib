Shader "Custom/SelfIluminOverlay" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Tile ("Tiling and Offset", Vector) = (1,1,0,0)
	}
	SubShader {
	  Tags {  "RenderType" = "Overlay" "Queue" = "Overlay" }
		LOD 200
		ZWrite  Off
		ZTest  Always
//		Cull Off
		Lighting Off
		
		CGPROGRAM
		#pragma surface surf Lambert

		struct Input {
			float4 screenPos;
		};

		sampler2D _MainTex;
		float4 _Color;
		float4 _Tile;

		void surf (Input IN, inout SurfaceOutput o) {
			float scXpY = _ScreenParams.x / _ScreenParams.y;
			float2 spos = float2((IN.screenPos.x-_Tile.z)*_Tile.x*scXpY,(IN.screenPos.y-_Tile.w)*_Tile.y)/IN.screenPos.w;
			half4 c = tex2D (_MainTex, spos);
//			o.Emission  = c.rgb*_Color.rgb;
			o.Emission = c.rgb+((_Color.rgb-0.5)*2.0);
//			o.Alpha = _Color.a*c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
