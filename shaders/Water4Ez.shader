Shader "FX/Water4Ez"
{
	Properties
	{
		_BubbleColor("Bubble color", COLOR) = (.8, .8, .8, 1)
		_WaterColor("Water color", COLOR) = (.1, .1, .4, 1)
		_MainTex("Fallback texture", 2D) = "black" {}
		_BumpMap("Normals ", 2D) = "bump" {}
		_WaterScale ("WaterScale", Range (0.01, 1.0)) = 0.1
		_GSteepness("Wave Steepness", Vector) = (1.0, 1.0, 1.0, 1.0)
		_GAmplitude("Wave Amplitude", Vector) = (0.3 ,0.35, 0.25, 0.25)
		_GFrequency("Wave Frequency", Vector) = (1.3, 1.35, 1.25, 1.25)
		_GSpeed("Wave Speed", Vector) = (1.2, 1.375, 1.1, 1.5)
		_GDirectionAB("Wave Direction", Vector) = (0.3 ,0.85, 0.85, 0.25)
		_GDirectionCD("Wave Direction", Vector) = (0.1 ,0.9, 0.5, 0.5)
	}
		CGINCLUDE
#include "UnityCG.cginc"
#include "WaterInclude.cginc"

		struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};

		half4 _BubbleColor;
		half4 _WaterColor;
		sampler2D _MainTex;
		float4 _MainTex_ST;
		sampler2D _BumpMap;
		float4 _BumpMap_ST;
		float _WaterScale;
		
		uniform float4 _GAmplitude;
		uniform float4 _GFrequency;
		uniform float4 _GSteepness;
		uniform float4 _GSpeed;
		uniform float4 _GDirectionAB;
		uniform float4 _GDirectionCD;

// shortcuts		
#define VERTEX_WORLD_NORMAL i.normalInterpolator.xyz
#define PER_PIXEL_DISPLACE (1) //_DistortParams.x

		struct v2f
		{
			float3 uv : TEXCOORD0;
			float4 bumpCoords : TEXCOORD1;
			UNITY_FOG_COORDS(2)
			float4 vertex : SV_POSITION;
			float4 normalInterpolator : TEXCOORD3;
			float3 viewInterpolator : TEXCOORD4;
		};

		v2f vert(appdata v)
		{
			v2f o;

			half3 worldSpaceVertex = mul(unity_ObjectToWorld, (v.vertex)).xyz;
			half3 vtxForAni = (worldSpaceVertex).xzz;
			half3 nrml;
			half3 offsets;
			
			half4 bumpDirection = float4(1.0, 1.0, -1.0, 1.0); // direction & speed
			half4 bumpTiling = float4(1.0, 1.0, -2.0, 3.0);
			half2 tileableUv = worldSpaceVertex.xz;
			o.bumpCoords.xyzw = (tileableUv.xyxy + _Time.xxxx * bumpDirection.xyzw) * bumpTiling.xyzw;

			Gerstner(
				offsets, nrml, v.vertex.xyz, vtxForAni,						// offsets, nrml will be written
				_GAmplitude,												// amplitude
				_GFrequency,												// frequency
				_GSteepness,												// steepness
				_GSpeed,													// speed
				_GDirectionAB,												// direction # 1, 2
				_GDirectionCD												// direction # 3, 4
				);

			v.vertex.xyz += offsets;

//			o.vertex = UnityObjectToClipPos(v.vertex);
			o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);

			float d = abs(sin(_Time.x));
			o.uv = float3((worldSpaceVertex.xz+ worldSpaceVertex.yy)*_WaterScale,d); // TRANSFORM_TEX(v.uv, _MainTex);

			o.viewInterpolator.xyz = worldSpaceVertex - _WorldSpaceCameraPos;
			o.normalInterpolator.xyz = nrml;
			o.normalInterpolator.w = 1;//GetDistanceFadeout(o.screenPos.w, DISTANCE_SCALE);

			UNITY_TRANSFER_FOG(o, o.vertex);
			return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{
			/*
			half3 worldNormal = PerPixelNormal(_BumpMap, i.bumpCoords, VERTEX_WORLD_NORMAL, PER_PIXEL_DISPLACE);
			half3 viewVector = normalize(i.viewInterpolator.xyz);
			fixed4 bunpCol = tex2D(_BumpMap, i.bumpCoords.xy * 10);
			bunpCol.rgb *= abs(dot(worldNormal, viewVector));
			*/

			// sample the texture
			fixed4 col = tex2D(_MainTex, i.uv.xy) * i.uv.z + tex2D(_MainTex, 1-i.uv.xy) * (1-i.uv.z); //tex2D(_BumpMap, i.bumpCoords);
			col.rgb = col.rgb * _BubbleColor.rgb + (1 - col.rgb)*_WaterColor.rgb;

			// apply fog
			UNITY_APPLY_FOG(i.fogCoord, col);
			return col;
		}

		ENDCG

		SubShader
		{
			Tags{ "RenderType" = "Opaque" }
			LOD 100

			Pass
			{
			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
			// make fog work
#pragma multi_compile_fog
#pragma multi_compile WATER_VERTEX_DISPLACEMENT_ON WATER_VERTEX_DISPLACEMENT_ON
			ENDCG
			}
		}
}
