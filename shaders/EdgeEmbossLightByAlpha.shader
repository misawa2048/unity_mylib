Shader "GUI/EdgeEmbossLightByAlpha" {
	Properties {
		_Color ("MainColor", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_FontTex ("Font (RGB)", 2D) = "white" {}
		_Offset ("Check Offset (UV)", Vector)=(0.01,0.01,0,0)
		_Bright("Brightness" ,Range(0.0,1.0))=0.5
	}
	
SubShader {
		Tags { "RenderType"="Opaque" "Queue"="Geometry" "IgnoreProjector"="true" }
	Pass {
//		AlphaTest Greater 0.5
		ZTest Less
		ZWrite On
		Lighting On
//		ColorMask RGBA
		Blend SrcAlpha OneMinusSrcAlpha

		
		CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members ofs,lightDir)
#pragma exclude_renderers d3d11 xbox360
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"
		
		sampler2D _MainTex;
		sampler2D _FontTex;
		half4 _Color;
		half4 _Offset;
		half _Bright;
		
		struct appdata_custom {
		    float4 vertex : POSITION;
		    float2 texcoord : TEXCOORD0;
		    float2 texcoord1 : TEXCOORD1;
		    float4 color : COLOR;
		};
		
		struct v2f {
		    float4  pos : SV_POSITION;
		    float2  uv : TEXCOORD0;
		    float2  uv1 : TEXCOORD1;
		    float4 color : COLOR;
		    float3 ofs;
		};
		
		float4 _MainTex_ST;
		float4 _FontTex_ST;
		
		v2f vert (appdata_custom v)
		{
		    v2f o;
		    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
		    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
		    o.uv1 = TRANSFORM_TEX (v.texcoord1, _FontTex);
		    o.color = v.color+unity_LightColor[0] + UNITY_LIGHTMODEL_AMBIENT;
		    o.ofs = _Offset * normalize(ObjSpaceLightDir(v.vertex));
		    return o;
		}
		
		half4 frag (v2f i) : COLOR
		{
			half a = tex2D (_FontTex, i.uv1).a;
			half ae = tex2D (_FontTex, i.uv1+i.ofs).a*_Bright;
			half ao = tex2D (_FontTex, i.uv1-i.ofs).a*_Bright;
			half4 c = tex2D (_MainTex, i.uv)*i.color;
			half4 outCol = (a-ae+ao)*c*_Color;
			outCol.a = a * _Color.a;
			
			return outCol;
		}
		ENDCG
	} 
}
	FallBack "Unlit"
}
