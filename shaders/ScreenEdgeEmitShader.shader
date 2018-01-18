Shader "Custom/ScreenEdgeEmitShader" {
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		[HDR] _Color ("Emission Color", Color) = (0,0,0)
        _EdgeWidth ("Edge Width", Range(0, 10)) = 1
    	_Sensitivity ("Depth Sensitivity", Range(0, 10)) = 1
    }
    CGINCLUDE
    #include "UnityCG.cginc"
    sampler2D _MainTex;
    sampler2D_float _CameraDepthTexture;
    float4 _Color;
    float _EdgeWidth;
    float _Sensitivity;

    struct v2f {
        float2 uv : TEXCOORD0;
    };

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

    fixed4 frag (v2f i, UNITY_VPOS_TYPE screenPos : VPOS) : SV_Target
    {
        float z0 = edgeCheck(screenPos.xy/(_ScreenParams.xy));
        fixed4 c = tex2D (_MainTex, i.uv);
		c.rgb += z0*_Color.rgb;
        return c;
    }
    ENDCG

    SubShader
    {
		Tags { "RenderType"="Opaque" }
		LOD 100
        ZTest Always Cull Off ZWrite Off
        Pass
        {
            CGPROGRAM
			#pragma vertex vert_img
            #pragma fragment frag
            #pragma target 3.0
            ENDCG
        }
    }
}
