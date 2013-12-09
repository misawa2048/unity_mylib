Shader "GUI/EdgeOutlineByAlpha" {
	Properties {
		_Color ("MainColor", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "clear" {}
		_FontTex ("Font (RGB)", 2D) = "white" {}
		_Offset ("Offset(U,V,Z,Brightness)", Vector)=(0.01,-0.01,0.0,0.5)
	}
SubShader {
		Tags { "RenderType"="Opaque" "Queue"="Geometry" "IgnoreProjector"="true" }
	Pass {
//		AlphaTest Greater 0.5
		ZTest Less
		ZWrite On
//		Lighting On
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
		    float2 ofs;
		};
		
		float4 _MainTex_ST;
		float4 _FontTex_ST;
		
		v2f vert (appdata_custom v)
		{
		    v2f o;
		    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
		    o.pos.z += _Offset.z;
		    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
		    o.uv1 = TRANSFORM_TEX (v.texcoord1, _FontTex);
		    o.color = v.color * unity_LightColor[0] + UNITY_LIGHTMODEL_AMBIENT;
		    o.ofs = normalize(ObjSpaceLightDir(v.vertex))*_Offset.xy;
		    return o;
		}
		
		half4 frag (v2f i) : COLOR
		{
			half a = tex2D (_FontTex, i.uv1).a;
			half ae=0;
			ae += tex2D (_FontTex, i.uv1 + float2(_Offset.x,_Offset.y)).a;
			ae += tex2D (_FontTex, i.uv1 + float2(_Offset.x,_Offset.y)).a;
			ae += tex2D (_FontTex, i.uv1 + float2(-_Offset.x,_Offset.y)).a;
			ae += tex2D (_FontTex, i.uv1 + float2(_Offset.x,-_Offset.y)).a;
			ae += tex2D (_FontTex, i.uv1 + float2(-_Offset.x,-_Offset.y)).a;
			ae += tex2D (_FontTex, i.uv1 + float2(_Offset.x,0)).a;
			ae += tex2D (_FontTex, i.uv1 + float2(-_Offset.x,0)).a;
			ae += tex2D (_FontTex, i.uv1 + float2(0,_Offset.y)).a;
			ae += tex2D (_FontTex, i.uv1 + float2(0,-_Offset.y)).a;
			ae *= 0.125;

			half4 c = tex2D (_MainTex, i.uv)*i.color;
			half4 outCol = c*ae+_Color*(1-ae)*_Offset.w;
			outCol.a = c.a*ae+a*_Color.a*(1-ae)*_Offset.w;
			
			return outCol;
		}
		ENDCG
	} 
}

	FallBack "Diffuse"
}
