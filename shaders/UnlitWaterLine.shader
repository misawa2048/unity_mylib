// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/WaterLine"
{
	Properties
	{
		_Color("MainColor", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_SurfaceHeight("surface height", float) = 0
		_LineSize("Line size", float) = 0.025
		_WaveAmp("Wave amp", float) = 5
	}
	SubShader
	{
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
		LOD 100
		Blend One One //SrcAlpha OneMinusSrcAlpha
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
				float3 normal : NORMAL0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float4 wPos : W_POS;
				float ofsHeight : OFS_H;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			float _SurfaceHeight;
			float _LineSize;
			float _WaveAmp;
			
			v2f vert (appdata v)
			{

				v2f o;
				o.wPos = mul(unity_ObjectToWorld, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				half4 COS = cos(_Time.yzyz*5 + v.vertex.xxzz*_WaveAmp);
				o.ofsHeight = (COS.x + COS.z)*_LineSize*_WaveAmp;
				v.vertex.y += o.ofsHeight;
				o.vertex = UnityObjectToClipPos(v.vertex + v.normal*_LineSize*_WaveAmp*2);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				clip(-i.wPos.y + _SurfaceHeight + _LineSize);
				clip( i.wPos.y - _SurfaceHeight + _LineSize);
			// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv)*_Color;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
