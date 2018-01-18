Shader "Custom/ModelEdgeEmitShader" {
	Properties {
		[HDR] _Color ("Emission Color", Color) = (2,2,2)
        _EdgeWidth ("Edge Width", Range(0, 10)) = 1
    	_Sensitivity ("Depth Sensitivity", Range(0, 100)) = 1
	}

    SubShader
    {
		Tags { "RenderType"="Transparent" "Queue"="Transparent+100"}
		LOD 200
		ZTest LEqual Cull Back ZWrite Off

		CGPROGRAM
		#include "UnityCG.cginc"
		#pragma surface surf Standard fullforwardshadows alpha
		#pragma target 3.0

		struct Input {
			float4 screenPos;
		};

    	sampler2D_float _CameraDepthTexture;
//		sampler2D_float _LastCameraDepthTexture;
		float4 _Color;
    	float _EdgeWidth;
    	float _Sensitivity;

		UNITY_INSTANCING_BUFFER_START(Props)
		// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

    	float edgeCheck(float2 i_uv){
    		float2 txSize = 1/_ScreenParams.xy*_EdgeWidth;
    		float2 base_uv = i_uv - txSize*0.5;
		    float2 uv0 = base_uv;
		    float2 uv1 = base_uv + txSize.xy;
		    float2 uv2 = base_uv + float2(txSize.x, 0);
		    float2 uv3 = base_uv + float2(0, txSize.y);

        	// Convert to linear depth values.
        	float z0 = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv0));
        	float z1 = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv1));
        	float z2 = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv2));
        	float z3 = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv3));

        	// Roberts cross operator
        	float zg1 = z1 - z0;
        	float zg2 = z3 - z2;
			return saturate(sqrt(zg1 * zg1 + zg2 * zg2)*_Sensitivity);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
        	float2 uv0 = IN.screenPos.xy / IN.screenPos.w;
        	uv0.x *= _ScreenParams.w;
        	float z0 = edgeCheck(uv0);
			o.Emission = _Color.rgb*z0;
			o.Alpha = z0*_Color.a;
		}
    	ENDCG
    }

	Fallback Off
}

