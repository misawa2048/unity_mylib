Shader "Unlit/CorrectARCameraShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "black" {}
		[Toggle] _FlipX("X Flip", Float) = 0
		[Toggle] _FlipY("Y Flip", Float) = 0
		_Brightness ("Brightness", Range (-1, 1)) = 0
		_Strech ("Strech", Vector)=(1,1,0,0)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

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
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _FlipX;
			float _FlipY;
			float _Brightness;
			float4 _Strech;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				if(_FlipX>0) o.uv.x = 1-o.uv.x;
				if(_FlipY>0) o.uv.y = 1-o.uv.y;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 duv = i.uv + float2(sin(i.uv.x*3.1416*2)*_Strech.z,sin(i.uv.y*3.1416*2)*_Strech.w);
				float2 stuv = 0.5+(duv-0.5)/_Strech.xy;
				// sample the texture
				fixed4 col = tex2D(_MainTex, stuv.xy);
				col.rgb += _Brightness;
				return col;
			}
			ENDCG
		}
	}
}
