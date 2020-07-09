//_WorldSpaceCameraPos

Shader "Unlit/PortalByCamera"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
	[HDR] _Color("Base Color", Color) = (1,1,1,0.5)
		_MaxDist("Max distance", Range(1,10))=5
		_RimPower("Rim power", Range(1,30)) = 5
		_Jitter("jitter power", Range(0,0.3)) = 0.1
	}
    SubShader
    {
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		//ColorMask RGB
		Cull Off Lighting Off ZWrite Off

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
				float3 wPos : TEXCOORD1;
			};

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float4 _Color;
			float _MaxDist;
			float _RimPower;
			float _Jitter;

			//-----------------------------------------------------------------------------
			//! 点にもっとも近い直線上の点(p1!=p2)
			//-----------------------------------------------------------------------------
			float3 NearestPointOnLine(float3 p1, float3 p2, float3 p) {
				float3 d = p2 - p1;
				//if (d.sqrMagnitude == 0.0f)    return p1;
				float t = (d.x * (p - p1).x + d.y * (p - p1).y + d.z * (p - p1).z) / length(d);
				return (1.0 - t)*p1 + t * p2;
			}

			v2f vert (appdata v)
            {
                v2f o;
				o.wPos = mul(unity_ObjectToWorld, v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float3 forward = mul((float3x3)unity_CameraToWorld, float3(0, 0, 1));
				float3 lPos = NearestPointOnLine(_WorldSpaceCameraPos, _WorldSpaceCameraPos - forward, i.wPos);
				float d = distance(i.wPos , lPos)- _MaxDist;
				float d2 = distance(i.wPos, _WorldSpaceCameraPos);
				float d3 = sin(i.wPos.x*2 + i.wPos.y*3 + i.wPos.z*4+_Time.w*3)*_Jitter;
				d = saturate(d+d2+d3);
				d = pow(d, _RimPower);
                // sample the texture
				float2 jit = float2(sin(i.wPos.x*3 + _Time.x), sin(i.wPos.y*5 + _Time.y))*0.03;
                fixed4 col = tex2D(_MainTex, i.uv+jit)*_Color;
				col.a *= d;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
            }

            ENDCG
        }
    }
}
