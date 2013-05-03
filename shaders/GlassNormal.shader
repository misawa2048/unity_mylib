Shader "Custom/GlassNormal" {
 Properties {
 _Color ("Main Color", Color) = (1,1,1)
 _MainTex ("Texture", 2D) = "gray" {}
 _ScrollX ("Scroll X", float) = 0.005
 _ScrollY ("Scroll Y", float) = 0.005
 _Convergence ("Specular Convergence" , Float) = 30.0
 }
 SubShader {
 Tags { "RenderType" = "Opaque" "Queue" = "Transparent" }
 Cull Back
 ZWrite On
 
CGPROGRAM
 #pragma surface surf GlassSpecular alpha
 
float _Convergence;
 half4 LightingGlassSpecular (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
 float nl = max(0.7, dot(s.Normal, lightDir)) * max(0.9,dot(viewDir, lightDir));
 half4 c;
 c.rgb = (_LightColor0.rgb * pow( nl,_Convergence) ) * atten * 100 ;
 c.a = s.Alpha;
 return c;
 }
 
struct Input {
 float2 uv_MainTex;
 float4 screenPos;
 float3 viewDir;
 float3 worldNormal;
 };
 float4 _Color;
 sampler2D _MainTex;
 float _ScrollX;
 float _ScrollY;
 void surf (Input IN, inout SurfaceOutput o) {
 float texAlpha = tex2D (_MainTex, IN.uv_MainTex).a * _Color.a;
 half NdotL = dot (IN.worldNormal, IN.viewDir);
 float2 screenUV = (IN.screenPos.xy+float2(_ScrollX*NdotL,_ScrollY*NdotL))/IN.screenPos.w;
 o.Emission = tex2D (_MainTex, screenUV).rgb * _Color;
 o.Alpha = texAlpha;
 }
 ENDCG
 }
 Fallback "Diffuse"
 }
