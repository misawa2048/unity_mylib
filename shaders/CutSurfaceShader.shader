Shader "Custom/CutSurfaceShader"
{
    Properties
    {
		_Section ("Section plane (x, y, z, and w displacement)", vector) = (0,-1,0,0)
        [HDR] _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		[HDR] _InnerColor ("InnerColor", Color) = (0,0,0,0)
        _InnerTex ("Inner Texture", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderQueue"="Geometry-10"}
        LOD 200
        CULL Off
        Offset 0 , 10

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows addshadow

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        float4 _Section;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			float3 calc = _Section.xyz * IN.worldPos;
			float toClip = calc.x + calc.y + calc.z + _Section.w;
			clip(-toClip);
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG

pass{
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
		
        sampler2D _InnerTex;
        float4 _InnerTex_ST;

		float4 _Section;
		half4 _InnerColor;
		
		struct appdata_custom {
		    float4 vertex : POSITION;
		    float3 normal : NORMAL;
            float2 uv : TEXCOORD0;
		};
		struct v2f {
		    float4  pos : SV_POSITION;
            float2 uv : TEXCOORD0;
		    float4 normal : NORMAL;
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
            o.uv = TRANSFORM_TEX(v.uv, _InnerTex);
            UNITY_TRANSFER_FOG(o,o.pos);
//			o.viewdir = normalize(WorldSpaceViewDir(v.vertex));
		    return o;
		}
		
		float4 frag (v2f i) : COLOR
		{
			float4 calc = _Section *i.wpos;
			float toClip = calc.x + calc.y + calc.z + calc.w;
			clip(-toClip);

            fixed4 outCol = tex2D(_InnerTex, i.uv)*_InnerColor;
            // apply fog
            UNITY_APPLY_FOG(i.fogCoord, outCol);
			return outCol;
		}
		ENDCG
}
    }
    FallBack "Diffuse"
}
