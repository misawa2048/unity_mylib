Shader "Unlit/Aurora"
{
	Properties
	{
		_Color ("MainColor", Color) = (1,1,1,1)
		_RimColor ("RimColor", Color) = (0.5,0.5,0.5,1)
		_MainTex ("Texture", 2D) = "white" {}
		_AlphaTex ("Alpha", 2D) = "white" {}
		_AlphaMin ("alpha min", float) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		LOD 100
		Blend One One //SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		Cull Off

		Pass
		{
			CGPROGRAM
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members dotL)
//#pragma exclude_renderers d3d11
			#pragma vertex vert
			#pragma fragment frag


			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float3 normal : NORMAL0;
				float3 wNormal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float4 vertex : SV_POSITION;
				float dotL: DOTL;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _AlphaTex;
			float4 _AlphaTex_ST;
			float4 _Color;
			float4 _RimColor;
			float _AlphaMin;

    half4 _GerstnerOffset4 (out float3 nrml, float3 vtx, half4 freq, half4 speed) 
    {
        half4 offsets; // xtx.xyz,normal.dot

        half4 TIME = _Time.yzyz * speed;

        half4 COS = cos (TIME+vtx.xxzz*freq);
        half4 SIN = sin (TIME+vtx.xxzz*freq);

        offsets.x = 0;
        offsets.y = SIN.x+SIN.y;
        offsets.z = 0;
        offsets.w = 0;
    	nrml = (float3((COS.x+COS.y)*0.5,1,0));

        return offsets;
    }   

			v2f vert (appdata v)
			{
				half3 nrml=0;
	    		half3 vtxOfs=0;
				vtxOfs = _GerstnerOffset4(nrml,v.vertex.xyz,
				half4(0.4,1,1,1), //freq
				half4(1,2,1,1)// speed
				);
    v.normal = nrml;
    v.vertex.xyz += vtxOfs*3;

				v2f o;
	float3 viewDir = normalize(UNITY_MATRIX_IT_MV[2].xyz); //mul(float4(0,0,1,0),UNITY_MATRIX_IT_MV)
    o.dotL = 0.5+dot (viewDir, v.normal);
    o.dotL = pow(o.dotL,4);
    o.dotL = o.dotL * (1-_AlphaMin);

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv2 = TRANSFORM_TEX(v.uv2, _AlphaTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float4 color = (_Color*i.dotL + _RimColor*(1-i.dotL));
				clip(color.a);
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				col *= tex2D(_AlphaTex, i.uv2);
				col *= color * max(color.a,0);
				return col;
			}

			ENDCG
		}
	}
}
