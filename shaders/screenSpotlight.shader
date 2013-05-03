Shader "Custom/screenspotlight" {
 Properties {
    _Color ("Main Color", Color) = (1,1,1)
   _MainTex ("Base (RGB)", 2D) = "white" {}
 }
 SubShader {
  Tags { "RenderType"="Opaque" }
//  Tags {  "RenderType" = "Opaque" "Queue" = "Transparent" }
 //  LOD 200
//  ZWrite  Off
//  Cull Off
pass{  
  CGPROGRAM
// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it does not contain a surface program or both vertex and fragment programs.
#pragma exclude_renderers gles
   #pragma fragment frag
//  #pragma surface surf Lambert
//  struct Input { float4 screenPos; };
//  void surf (Input IN, inout SurfaceOutput o) {
//   float2 ofs = (0.5,0.5)-(IN.screenPos.xy / IN.screenPos.w);
 //   o.Emission  = (length(ofs)>0.5 ? 0 : pow(0.5-length(ofs),0.5));
//  }

  struct v2f {
      float4 pos : SV_POSITION;
      float4 uv : TEXCOORD0;
   };
  
  fixed4 frag (v2f i) : COLOR0 {
   half4 c = frac( i.uv );
   float2 ofs = (0.5,0.5)-(i.uv.xy);
//   c.r = ofs.x;
 //  c.b = ofs.y;
   return c; //(length(ofs)>0.5 ? 0 : pow(0.5-length(ofs),0.5));
   }
  ENDCG
}
 } 
 FallBack "Diffuse"
}