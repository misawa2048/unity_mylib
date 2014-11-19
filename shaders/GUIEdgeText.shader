Shader "GUI/EdgeText"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_Edge ("Edge Color", Color) = (0,0,0,1)
		_MainTex ("Alpha (A)", 2D) = "white" {}
		_Offset ("Offset(U,V,Z,Brightness)", Vector)=(0.01,-0.01,0.0,0.5)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_ColorMask ("Color Mask", Float) = 15
	}

	SubShader
	{
		LOD 200

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}

		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Offset -1, -1
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
			CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members ofs)
#pragma exclude_renderers d3d11 xbox360
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				struct appdata_t
				{
					float4 vertex : POSITION;
					half4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex : POSITION;
					half4 color : COLOR;
					float2 texcoord : TEXCOORD0;
					float2 ofs;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				fixed4 _Color;
				fixed4 _Edge;
				half4 _Offset;

				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.color = v.color;
					o.ofs = _Offset.xy;
					return o;
				}

				half4 frag (v2f i) : COLOR
				{
			half a = tex2D (_MainTex, i.texcoord).a;
			a += tex2D (_MainTex, i.texcoord+i.ofs).a;
			a += tex2D (_MainTex, i.texcoord-i.ofs).a;
			a += tex2D (_MainTex, i.texcoord+i.ofs * float2(1,-1)).a;
			a += tex2D (_MainTex, i.texcoord-i.ofs * float2(1,-1)).a;
			a *= 0.2;
			a = clamp((0.5-abs(a-0.5))*_Offset.w,0,1);
			half4 outCol;
					half4 col = i.color*(1-a)+_Edge*a;
					col.a *= (tex2D(_MainTex, i.texcoord).a);
					col = col * _Color;
					clip (col.a - 0.01);
					return col;
				}
			ENDCG
		}
	}
}
