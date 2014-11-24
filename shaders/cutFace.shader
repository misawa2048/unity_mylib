Shader "Custom/cutFace" {
	Properties {
		_Section ("Section plane (x, y, z, and w displacement)", vector) = (0,-1,0,0)
		_Color ("MainColor", Color) = (1,1,1,1)
	}
SubShader {
	Tags {  "RenderType" = "Opaque" "Queue" = "Geometry" }
	Pass {
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask RGB
	    ZWrite On
    	ZTest LEqual //Always
//    	AlphaTest Greater 0.5
    	Cull Back
		
		CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members g0,bw)
#pragma exclude_renderers d3d11 xbox360
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"
		
		float4 _Section;
		half4 _Color;
		
		struct appdata_custom {
		    float4 vertex : POSITION;
		    float3 normal : NORMAL;
		};
		struct v2f {
		    float4  pos : SV_POSITION;
		    float4 normal : TEXCOORD0;
		    float4  wpos :TEXCOORD1;
//		    float3 viewdir : TEXCOORD2;
		};
		
		v2f vert (appdata_custom v)
		{
		    v2f o;
		    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
		    o.wpos = mul (_Object2World, v.vertex);
			o.normal.xyz = mul((float3x3)_Object2World, v.normal);
			o.normal.w=dot(v.normal,normalize(ObjSpaceViewDir(v.vertex)));
//			o.viewdir = normalize(WorldSpaceViewDir(v.vertex));
		    return o;
		}
		
		float4 frag (v2f i) : COLOR
		{
			_Section.xyzw *= 0.1*i.wpos.xyzw;
			float toClip = _Section.x + _Section.y + _Section.z + _Section.w;
			clip( toClip);

			half4 outCol;
			outCol.rgb = 0;
			outCol.a = 1; //abs(i.normal.w);
			return outCol;
		}
		ENDCG
	}
	
	Pass {
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask RGB
	    ZWrite On
    	ZTest LEqual //Always
//    	AlphaTest Greater 0.5
    	Cull Front
		
		CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members g0,bw)
#pragma exclude_renderers d3d11 xbox360
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"
		
		float4 _Section;
		half4 _Color;
		
		struct appdata_custom {
		    float4 vertex : POSITION;
		    float3 normal : NORMAL;
		};
		struct v2f {
		    float4  pos : SV_POSITION;
		    float4 normal : TEXCOORD0;
		    float4  wpos :TEXCOORD1;
//		    float3 viewdir : TEXCOORD2;
		};
		
		v2f vert (appdata_custom v)
		{
		    v2f o;
		    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
		    o.wpos = mul (_Object2World, v.vertex);
			o.normal.xyz = mul((float3x3)_Object2World, v.normal);
			o.normal.w=dot(v.normal,normalize(ObjSpaceViewDir(v.vertex)));
//			o.viewdir = normalize(WorldSpaceViewDir(v.vertex));
		    return o;
		}
		
		float4 frag (v2f i) : COLOR
		{
			_Section.xyzw *= 0.1*i.wpos.xyzw;
			float toClip = _Section.x + _Section.y + _Section.z + _Section.w;
			clip( toClip);

			half4 outCol;
			outCol.rgb = _Color.rgb;
			outCol.a = 1; //abs(i.normal.w);
			return outCol;
		}
		ENDCG
	}
	
	} 
	FallBack "Diffuse"
}
