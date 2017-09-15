// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/EquirectangularPlusShader"
{
	Properties
	{
		_MainTex ("CubeMap", CUBE) = "white" {}
		_Rotarion ("Rotation", Vector) = (1, 0, 0, 0)
		_Mode ("Mode", Int) = 0
		_Zoom ("Zoom", Float) = 0
		_Brightness ("Brightness", Float) = 1
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
			
			samplerCUBE _MainTex;
			float4 _Rotation;
			int _Mode;
			float _Zoom;
			float _Brightness;

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
				#ifdef USE_DOMEMODE
				o.uv = o.uv*(1-_Zoom)+_Zoom*0.5;
				#else
				o.uv.y -= _Zoom;
				#endif
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float2 coord;
				float3 pos;

				#ifndef USE_DOMEMODE
				if(i.uv.y<0){ return fixed4(0, 0, 0, 1); }
				if(i.uv.y>1){ return fixed4(0, 0, 0, 1); }
				#endif

				#ifdef IS_SQUARE
				#ifdef USE_DOMEMODE
				if(i.uv.x<_Zoom*0.5){ return fixed4(0, 0, 0, 1); }
				if(i.uv.x>(1-_Zoom*0.5)){ return fixed4(0, 0, 0, 1); }
				#else
				if(i.uv.x<0){ return fixed4(0, 0, 0, 1); }
				if(i.uv.x>1){ return fixed4(0, 0, 0, 1); }
				#endif
				#endif

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
//					clip(coord.y < 0); // いらない部分をクリップ
					if(coord.y < _Zoom*3.14*0.5){ return fixed4(0, 0, 0, 1); } // _Zoom
					pos = float3(-0.5+i.uv.x, sin(coord.y), 0.5-i.uv.y);
					pos.xz *= sqrt(1 - pos.y * pos.y) / distance(pos.xz, 0);
				#endif

				// apply camera rot (http://www.geeks3d.com/20141201/how-to-rotate-a-vertex-by-a-quaternion-in-glsl/)
				pos = pos + 2.0 * cross(_Rotation.xyz, cross(_Rotation.xyz, pos) + _Rotation.w * pos);

				fixed4 col = texCUBE(_MainTex, pos)*_Brightness;

				return col;
			}
			ENDCG
		}

	}
}
