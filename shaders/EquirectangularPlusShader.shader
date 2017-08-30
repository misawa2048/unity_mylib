// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/EquirectangularPlusShader"
{
	Properties
	{
		_MainTex ("CubeMap", CUBE) = "white" {}
		_Rotarion ("Rotation", Vector) = (1, 0, 0, 0)
		_Mode ("Mode", Int) = 0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		CGINCLUDE
		#pragma multi_compile _ USE_HFLIP
		#pragma multi_compile _ USE_DOMEMODE
		#pragma multi_compile _ IS_SQUARE

		#include "UnityCG.cginc"

		ENDCG

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				#ifdef IS_SQUARE
					float rate = _ScreenParams.x/_ScreenParams.y;
					if(rate>1){
						o.uv.x = (1-rate)*0.5+ v.uv.x*rate;
						o.uv.y = v.uv.y;
					}else{
						rate = 1/rate;
						o.uv.x = v.uv.x;
						o.uv.y = (1-rate)*0.5+ v.uv.y*rate;
					}
				#else
					o.uv = v.uv;
				#endif
				return o;
			}

			float3 rotation(float4 q, float3 vec)
			{
				return 2.0 * dot(q.xyz, vec) * q.xyz + (q.w*q.w - dot(q.xyz, q.xyz)) * vec + 2.0 * q.w * cross(q.xyz, vec);
			}
			
			samplerCUBE _MainTex;
			float4 _Rotation;
			int _Mode;

			fixed4 frag (v2f i) : SV_Target
			{
				float2 coord;
				float3 pos;

				#ifdef USE_HFLIP
					i.uv.x = 1-i.uv.x;
				#endif

				#ifndef USE_DOMEMODE
				// Equirectangular Mode
					coord.x = i.uv.x * 2.0 * UNITY_PI + UNITY_PI;
					coord.y = (i.uv.y - 0.5) * UNITY_PI;
					pos = float3(sin(coord.x), sin(coord.y), -cos(coord.x));
					pos.xz *= sqrt(1 - pos.y * pos.y);
				#else
				// Dome Master Mode
					coord.y = (0.5 - distance(i.uv, 0.5)) * UNITY_PI;
					pos = float3(-0.5+i.uv.x, sin(coord.y), 0.5-i.uv.y);
					pos.xz *= sqrt(1 - pos.y * pos.y) / distance(pos.xz, 0);
				#endif

				// apply camera rotation
				pos = rotation(_Rotation, pos);

				fixed4 col = texCUBE(_MainTex, pos);

				#ifdef USE_DOMEMODE
				// add black mask when DomeMaster mode.
//				if (_Mode == 1 && coord.y < 0) col = fixed4(0, 0, 0, 1);
				if (coord.y < 0) col = fixed4(0, 0, 0, 1);
				#endif

				return col;
			}
			ENDCG
		}

	}
}
