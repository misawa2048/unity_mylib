// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/cutFaceBack" {
	Properties {
		_Section ("Section plane (x, y, z, and w displacement)", vector) = (0,-1,0,0)
        _MainTex ("Texture", 2D) = "white" {}
		[HDR] _BaseColor ("BaseColor", Color) = (0,0,0,0)
		[HDR] _Color ("CutColor", Color) = (1,1,1,1)
	}
SubShader {
	Tags {  "RenderType" = "Opaque" "Queue" = "Geometry" }
	Pass {
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask 0 //RGB
	    ZWrite On
    	ZTest LEqual //Always
//    	AlphaTest Greater 0.5
    	Cull Back
        Offset 0 , 1
		
		CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members g0,bw)
#pragma exclude_renderers d3d11 xbox360
		#pragma vertex vert
		#pragma fragment frag
        // make fog work
        #pragma multi_compile_fog
		#include "UnityCG.cginc"
		
		float4 _Section;
		half4 _BaseColor;
        sampler2D _MainTex;
        float4 _MainTex_ST;

		
        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
        };
		struct v2f {
		    float4  pos : SV_POSITION;
            float2 uv : TEXCOORD0;
		    float4  wpos :TEXCOORD1;
            UNITY_FOG_COORDS(2)
//		    float3 viewdir : TEXCOORD2;
		};
		
		v2f vert (appdata v)
		{
		    v2f o;
		    o.pos = UnityObjectToClipPos (v.vertex);
		    o.wpos = mul (unity_ObjectToWorld, v.vertex);
            o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            UNITY_TRANSFER_FOG(o,o.pos);
//			o.viewdir = normalize(WorldSpaceViewDir(v.vertex));
		    return o;
		}
		
		float4 frag (v2f i) : COLOR
		{
			float4 calc = _Section *i.wpos;
			float toClip = calc.x + calc.y + calc.z + calc.w;
			clip(-toClip);

            fixed4 outCol = tex2D(_MainTex, i.uv)*_BaseColor;
			//outCol.a = 1;
            // apply fog
            UNITY_APPLY_FOG(i.fogCoord, outCol);
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
        Offset 0 , -1
		
		CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members g0,bw)
#pragma exclude_renderers d3d11 xbox360
            // make fog work
            #pragma multi_compile_fog
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
            UNITY_FOG_COORDS(2)
//		    float3 viewdir : TEXCOORD2;
		};
		
		v2f vert (appdata_custom v)
		{
		    v2f o;
		    o.pos = UnityObjectToClipPos (v.vertex);
		    o.wpos = mul (unity_ObjectToWorld, v.vertex);
			o.normal.xyz = mul((float3x3)unity_ObjectToWorld, v.normal);
			o.normal.w=dot(v.normal,normalize(ObjSpaceViewDir(v.vertex)));
            UNITY_TRANSFER_FOG(o,o.pos);
//			o.viewdir = normalize(WorldSpaceViewDir(v.vertex));
		    return o;
		}
		
		float4 frag (v2f i) : COLOR
		{
			float4 calc = _Section *i.wpos;
			float toClip = calc.x + calc.y + calc.z + calc.w;
			clip(-toClip);

			half4 outCol = _Color;
            // apply fog
            UNITY_APPLY_FOG(i.fogCoord, outCol);
			//outCol.a = 1; //abs(i.normal.w);
			return outCol;
		}
		ENDCG
	}
	
	} 
	FallBack "Diffuse"
}
