Shader "Unlit/Water4Line"
{
	Properties
	{
		_Color("MainColor", Color) = (1,1,1,1)
		_SurfaceHeight("water surface height", float) = 0
		_Freq("Wave Freq", float) = 1
		_WaveWidth("Wave Width", float) = 1
		_WaveAmp("Wave Amp", float) = 5
		_GSteepness("Wave Steepness", Vector) = (1.0, 1.0, 1.0, 1.0)
		_GAmplitude("Wave Amplitude", Vector) = (0.3 ,0.35, 0.25, 0.25)
		_GFrequency("Wave Frequency", Vector) = (1.3, 1.35, 1.25, 1.25)
		_GSpeed("Wave Speed", Vector) = (1.2, 1.375, 1.1, 1.5)
		_GDirectionAB("Wave Direction", Vector) = (0.3 ,0.85, 0.85, 0.25)
		_GDirectionCD("Wave Direction", Vector) = (0.1 ,0.9, 0.5, 0.5)
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
#pragma multi_compile WATER_VERTEX_DISPLACEMENT_ON WATER_VERTEX_DISPLACEMENT_ON

			#include "UnityCG.cginc"
			#include "WaterInclude.cginc"

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
				float surfHeight : WAVE_H;
			};

			float4 _Color;
			float _SurfaceHeight;
			float _Freq;
			float _WaveWidth;
			float _WaveAmp;
			uniform float4 _GAmplitude;
			uniform float4 _GFrequency;
			uniform float4 _GSteepness;
			uniform float4 _GSpeed;
			uniform float4 _GDirectionAB;
			uniform float4 _GDirectionCD;

			v2f vert (appdata v)
			{
				half4 wPos = mul(unity_ObjectToWorld, v.vertex);
				v2f o;
				o.wPos = wPos;
				o.vertex = UnityWorldToClipPos(wPos.xyz);

				// for wave
				half3 worldSpaceVertex = mul(unity_ObjectToWorld, v.vertex).xyz;
				half3 vtxForAni = (worldSpaceVertex).xzz;
				half3 nrml;
				half3 offsets;
				Gerstner(
					offsets, nrml, v.vertex.xyz, vtxForAni,						// offsets, nrml will be written
					_GAmplitude,												// amplitude
					_GFrequency,												// frequency
					_GSteepness,												// steepness
					_GSpeed,													// speed
					_GDirectionAB,												// direction # 1, 2
					_GDirectionCD												// direction # 3, 4
				);
				o.surfHeight = _SurfaceHeight + offsets.y*0.5;

				half4 COS = cos(_Time.yzyz*_Freq + wPos.xxzz*_WaveWidth);
				o.ofsHeight = ((COS.x + COS.z)-1)*_WaveAmp;

				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				clip( 0.5+i.wPos.y - i.surfHeight);
				clip(-i.wPos.y + i.surfHeight + _WaveAmp - i.ofsHeight);

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
