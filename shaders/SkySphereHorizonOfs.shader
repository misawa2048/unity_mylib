Shader "Unlit/SkySphereHorizonOfs"
{
	Properties
	{
		_FadeRate("Fade Rate", Range(0, 1)) = 0
		_Color ("Tex1 Color", Color) = (1,1,1,1)
		_Color2 ("Tex2 Color", Color) = (1,1,1,1)
		_Tex ("Texture", 2D) = "white" {}
		_Tex2 ("Texture2", 2D) = "white" {}
	}
	SubShader
	{
	Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
	Cull Front
	ZWrite Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _Tex;
			float4 _Tex_ST;
			uniform half4 _Color;
			sampler2D _Tex2;
			float4 _Tex2_ST;
			uniform half4 _Color2;
			uniform half _FadeRate;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _Tex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_Tex, i.uv)*(1-_FadeRate) * _Color+tex2D(_Tex2, i.uv)*_FadeRate * _Color2;
				return col;
			}
			ENDCG
		}
	}
}
