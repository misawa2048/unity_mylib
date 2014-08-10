Shader "Custom/ShadowAndDepth" {
 
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}
 
SubShader {
    Tags {"Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
    LOD 200
    Blend Zero SrcColor
    ZWrite On
    ZTest LEqual
//		Lighting Off
//		SeparateSpecular Off
 
CGPROGRAM
#pragma surface surf SimpleLambert noambient
 
fixed4 _Color;
sampler2D _MainTex;
struct Input {
    float2 uv_MainTex;
};
 
       half4 LightingSimpleLambert (SurfaceOutput s, half3 lightDir, half atten) {
              half NdotL = max(0.8,dot (s.Normal, lightDir));
              half4 c;
              c.rgb = s.Albedo * (_LightColor0.rgb+1) * (NdotL * atten * 2);
              c.a = s.Alpha;              
              return c;
          }
          
  void surf (Input IN, inout SurfaceOutput o) {
	half4 c = tex2D (_MainTex, IN.uv_MainTex);
//    fixed4 c = _Color;
    o.Albedo = c.rgb;
    o.Emission = 0;
//    o.Normal = 1;
    o.Alpha = c.a;
}

ENDCG
}
 
Fallback "Transparent/Cutout/VertexLit"
}