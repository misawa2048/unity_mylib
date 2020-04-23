Shader "Custom/SurfaceHideOnlyShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		[HDR] _Color ("Color", Color) = (1,1,1,1)
    }

    CGINCLUDE
        #include "UnityCG.cginc"
        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
            float3 normal : NORMAL;
        };

        struct v2f
        {
            float2 uv : TEXCOORD0;
            float4 vertex : SV_POSITION;
            float3 wnormal : TEXCOORD1;
            UNITY_FOG_COORDS(2)
        };

        sampler2D _MainTex;
        float4 _MainTex_ST;
        float4 _Color;

        v2f vert (appdata v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            o.wnormal = mul (unity_ObjectToWorld, v.normal);
            UNITY_TRANSFER_FOG(o,o.vertex);
            return o;
        }

        fixed4 frag (v2f i) : SV_Target
        {
            //if(dot(i.wnormal,float3(0,0,1))<0){ return(fixed4(0,0,0,1)); }
            // sample the texture
            fixed4 col = tex2D(_MainTex, i.uv)*_Color;
            // apply fog
            UNITY_APPLY_FOG(i.fogCoord, col);
            return col;
        }
    ENDCG

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="AlphaTest+10"}
        LOD 100
        CULL Front
        ZWrite Off
    	ZTest Greater
        //Offset 0 , 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            ENDCG
        }
    }
}
