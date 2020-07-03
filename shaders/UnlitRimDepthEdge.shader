Shader "Unlit/RimDepthEdge"
{
	Properties{
		_MainTex("Particle Texture", 2D) = "white" {}
		_InvFade("Soft Particles Factor", Range(2,100.0)) = 5.0
	}

		Category{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" }
		Blend One OneMinusSrcAlpha
		ColorMask RGB
		Cull Off Lighting Off ZWrite Off

		SubShader{
		Pass{

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 2.0
#pragma multi_compile_particles
#pragma multi_compile_fog

#include "UnityCG.cginc"

		sampler2D _MainTex;
	fixed4 _TintColor;

	struct appdata_t {
		float4 vertex : POSITION;
		fixed4 color : COLOR;
		float2 texcoord : TEXCOORD0;
		float3 normal : NORMAL;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2f {
		float4 vertex : SV_POSITION;
		fixed4 color : COLOR;
		float2 texcoord : TEXCOORD0;
		UNITY_FOG_COORDS(1)
		float4 projPos : TEXCOORD2;
		float3 vnormal : TEXCOORD3;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	float4 _MainTex_ST;

	v2f vert(appdata_t v)
	{
		v2f o;
		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.projPos = ComputeScreenPos(o.vertex);
		COMPUTE_EYEDEPTH(o.projPos.z);
		o.color = v.color;
		o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
		o.vnormal = COMPUTE_VIEW_NORMAL;
		UNITY_TRANSFER_FOG(o,o.vertex);
		return o;
	}

	UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
	float _InvFade;

	float depthEdge(float4 _projPos) {
		float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(_projPos)));
		float partZ = _projPos.z;
		float fade = saturate(_InvFade * (sceneZ - partZ) * 0.5) * 2; // 0-2
		fade = (1 - abs(1 - fade)); // 0-1-0
		return fade;
	}

	float rimEdge(float3 _vnormal) {
		float3 viewDir = float3(0, 0, 1); // UNITY_MATRIX_IT_MV[2].xyz;
		float fade = saturate((dot(viewDir, _vnormal)));
		if (fade > 0) {
			fade = 1 - fade;
		}
		return pow(fade,2);
	}

	fixed4 frag(v2f i) : SV_Target
	{
		float fade = depthEdge(i.projPos);
		fade += rimEdge(i.vnormal);
		i.color.a *= fade;

		half4 col = i.color * tex2D(_MainTex, i.texcoord);
		col.rgb *= col.a;
		UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,0)); // fog towards black due to our blend mode
		return col;
	}

		ENDCG
	}
	}
	}
}
