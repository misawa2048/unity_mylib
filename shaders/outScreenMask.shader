Shader "Custom/outScreenMask" {
 Properties {
  _MainTex ("Base (RGB)", 2D) = "white" {}
  _Color ("Color", Color) = (1,1,1,1)
  _ScreenSizeRate ("Screen size rate",Vector) = (1,1,0,0)
  }
 SubShader {
  Tags { "RenderType"="Opaque" "Queue"="Overlay" }
  LOD 200
  ZWrite  Off
  Lighting Off
  
  CGPROGRAM
  #pragma surface surf Lambert noambient novertexlights nolightmap noforwardadd 

  sampler2D _MainTex;
  float4 _Color;
  float4 _ScreenSizeRate;

  struct Input {
   float2 uv_MainTex;
   float4 screenPos;
  };

  void surf (Input IN, inout SurfaceOutput o) {
   half scXpY = _ScreenParams.x / _ScreenParams.y;
   half dtXpY = _ScreenSizeRate.x/_ScreenSizeRate.y;
   half rx = (_ScreenSizeRate.z-dtXpY/scXpY)*0.5;
   half ry = (_ScreenSizeRate.w-scXpY/dtXpY)*0.5;
   half2 spos = IN.screenPos/IN.screenPos.w;
   half clipXY;
   clipXY = step(0,spos.x-rx);
   clipXY += step(0,-spos.x+(1-rx));
   clipXY += step(0,spos.y-ry);
   clipXY += step(0,-spos.y+(1-ry));
   clip(3-clipXY);
   half4 c = tex2D (_MainTex, IN.uv_MainTex);
   o.Emission = c.rgb+((_Color.rgb-0.5)*5.0);
  }
  ENDCG
 } 
 FallBack "Diffuse"
 }
