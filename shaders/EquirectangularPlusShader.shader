// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/EquirectangularPlusShader"
{
	Properties
	{
		_MainTex ("CubeMap", CUBE) = "white" {}
		_Rotarion ("Rotation", Vector) = (1, 0, 0, 0)
		_Mode ("Mode", Int) = 0
		_Zoom ("Zoom", Float) = 0
		_Margin ("Margin", Float) = 0
		_Brightness ("Brightness", Float) = 1
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		CGINCLUDE
		#pragma multi_compile _ USE_HFLIP
		#pragma multi_compile _ USE_DOMEMODE
		#pragma multi_compile _ USE_ZOOM
		#pragma multi_compile _ USE_BRIGHTNESS
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
			float _Margin;
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
				o.uv = o.uv*(1+_Margin*2)-_Margin;
				#ifdef USE_ZOOM
				#ifdef USE_DOMEMODE
				o.uv = o.uv*(1-_Zoom)+_Zoom*0.5;
				#else
				o.uv.y -= _Zoom;
				#endif
				#endif // USE_ZOOM
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float2 coord;
				float3 pos;
				fixed4 baseCol = fixed4(0, 0, 0, 0);

				#ifndef USE_DOMEMODE
				if(i.uv.y<0){ return baseCol; }
				if(i.uv.y>1){ return baseCol; }
				#endif

				#ifdef IS_SQUARE
				#ifdef USE_DOMEMODE
				#ifdef USE_ZOOM
				if(i.uv.x<_Zoom*0.5){ return baseCol; }
				if(i.uv.x>(1-_Zoom*0.5)){ return baseCol; }
				#endif // USE_ZOOM
				#else
				if(i.uv.x<0){ return baseCol; }
				if(i.uv.x>1){ return baseCol; }
				#endif
				#endif

				#ifdef USE_HFLIP
					i.uv.x = 1-i.uv.x;
				#endif

				#ifndef USE_DOMEMODE
				// Equirectangular Mode
					coord.x = i.uv.x * 2.0 * UNITY_PI + UNITY_PI;
					coord.y = min(0.496,max(-0.496,(i.uv.y - 0.5)))*UNITY_PI; //0.496:for topbottomlinebug
					pos = float3(sin(coord.x), sin(coord.y), -cos(coord.x));
					pos.xz *= sqrt(1 - pos.y * pos.y);
				#else
				// Dome Master Mode
					coord.y = (0.5 - max(distance(i.uv, 0.5),0.004)) * UNITY_PI; //0.004:for centerposbug
//					clip(coord.y < 0); // いらない部分をクリップ
					#ifdef USE_ZOOM
					if(coord.y < _Zoom*3.14*0.5){ return baseCol; } // _Zoom
					#endif // USE_ZOOM
					pos = float3(-0.5+i.uv.x, sin(coord.y), 0.5-i.uv.y);
					pos.xz *= sqrt(1 - pos.y * pos.y) / distance(pos.xz, 0);
				#endif

				// apply camera rot (http://www.geeks3d.com/20141201/how-to-rotate-a-vertex-by-a-quaternion-in-glsl/)
				pos = pos + 2.0 * cross(_Rotation.xyz, cross(_Rotation.xyz, pos) + _Rotation.w * pos);
//				pos.y=sin(pos.y*0.5*UNITY_PI)*0.7;
				fixed4 col = texCUBE(_MainTex, pos);
				#ifdef USE_BRIGHTNESS
				col *= float4(_Brightness,_Brightness,_Brightness,1);
				float y =  0.3*col.r + 0.59*col.g + 0.11*col.b;
				col.rgb +=((1-y) * (_Brightness-1)*0.25)*col.a;
				#endif // USE_BRIGHTNESS
				return col;
			}
			ENDCG
		}

	}
}
