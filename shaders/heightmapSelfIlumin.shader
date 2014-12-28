Shader "Custom/heightmapSelfIlumin" {
	Properties {
		_Color ("MainColor", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_HeightTex ("Heightmap (RGB)", 2D) = "gray" {}
	}
SubShader {
	Tags { "RenderType"="Geometry" "Queue"="Geometry" "IgnoreProjector"="true" }
	Pass {
//		ZTest LEqual
		ZWrite On
		Lighting On
		Cull Front
		
		CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members ofs,lightDir)
//#pragma exclude_renderers d3d11 xbox360
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"
		
		sampler2D _MainTex : TEXCOORD0;
		sampler2D _HeightTex : TEXCOORD1;
		half4 _Color : COLOR;
		
		struct appdata_custom {
		    float4 vertex : POSITION;
		    float2 texcoord : TEXCOORD0;
		    float2 texcoord1 : TEXCOORD1;
		    float4 color : COLOR;
		    float3 normal : NORMAL;
		};
		
		struct v2f {
		    float4  pos : SV_POSITION;
		    float2  uv : TEXCOORD0;
		    float2  uv1 : TEXCOORD1;
		    float4  color : COLOR;
//		    float3 normal : NORMAL;
//		    float4  wpos : TEXCOORD2;
//		    float4  wnormal : TEXCOORD3;
		};
		
		float4 _MainTex_ST;
		float4 _HeightTex_ST;
		
		v2f vert (appdata_custom v)
		{
#if !defined(SHADER_API_OPENGL)
	float4 ofs = tex2Dlod(_HeightTex, float4(v.texcoord.xy,0,0));
	v.normal.xyz *= ofs.x;
#endif
		    v2f o;
		    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
		    o.uv1 = TRANSFORM_TEX (v.texcoord1, _HeightTex);
		    o.pos = mul (UNITY_MATRIX_MVP, v.vertex-float4(v.normal*0.5,0));
		    o.color = v.color * _Color;
//		    o.wpos = mul (_Object2World, v.vertex);
//		    o.wnormal.xyz = mul((float3x3)_Object2World, v.normal);
//			o.wnormal.w=dot(v.normal,normalize(ObjSpaceViewDir(v.vertex)));
		    return o;
		}
		
		half4 frag (v2f i) : COLOR
		{
//			half h = tex2D (_HeightTex, i.uv1).r;
			half4 outCol;
			outCol = tex2D (_MainTex, i.uv);
//			outCol.a = tex2D (_HeightTex, i.uv1).a;
			
			return outCol;
		}
		ENDCG
	} 
}
	FallBack "Diffuse"
}
