Shader "Unlit/UnlitRimColorShader"
{
	Properties
	{
		_Color ("MainColor", Color) = (1,1,1,1)
		_RimColor ("RimColor", Color) = (0.5,0.5,0.5,1)
		_MainTex ("Texture", 2D) = "white" {}
		_AlphaMin ("alpha min", float) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
				float3 wNormal : FLOAT;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float dotL : FLOAT;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			float4 _RimColor;
			float _AlphaMin;

			v2f vert (appdata v)
			{
				v2f o;
	float3 viewDir = normalize(UNITY_MATRIX_IT_MV[2].xyz);
    o.dotL = 0.5+dot (viewDir, v.normal*0.5);
    o.dotL = pow(o.dotL,4);
    o.dotL = o.dotL * (1-_AlphaMin);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) * (_Color*i.dotL + _RimColor*(1-i.dotL));
//				col.a = 1-i.dotL;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}

			ENDCG
		}
	}
}
