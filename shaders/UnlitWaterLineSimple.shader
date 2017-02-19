Shader "Unlit/WaterLineSimple"
{
	Properties
	{
		_Color("MainColor", Color) = (1,1,1,1)
		_SurfaceHeight("water surface height", float) = 0
		_Freq("Wave Freq", float) = 1
		_WaveWidth("Wave Width", float) = 1
		_WaveAmp("Wave Amp", float) = 5
	}
	SubShader
	{
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent+10" }
		LOD 100
		ZTest LEqual
		Offset 0,-10
		Blend ONE One //SrcAlpha OneMinusSrcAlpha
		ZWrite OFF

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
				float3 normal : NORMAL0;
			};

			struct v2f
			{
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float4 wPos : W_POS;
				float ofsHeight : OFS_H;
			};

			float4 _Color;
			float _SurfaceHeight;
			float _Freq;
			float _WaveWidth;
			float _WaveAmp;
			
			v2f vert (appdata v)
			{
				half4 wPos = mul(unity_ObjectToWorld, v.vertex);
				v2f o;
				o.wPos = wPos;
//				o.vertex = UnityObjectToClipPos(v.vertex);
				o.vertex = UnityWorldToClipPos(wPos.xyz);

				half4 COS = cos(_Time.yzyz*_Freq + wPos.xxzz*_WaveWidth);
				o.ofsHeight = ((COS.x + COS.z)-1)*_WaveAmp;

				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				clip( i.wPos.y - _SurfaceHeight);
				clip(-i.wPos.y + _SurfaceHeight + _WaveAmp - i.ofsHeight);

			// sample the texture
				fixed4 col = _Color;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
