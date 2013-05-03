Shader "Custom/SimpleGauge" {
  Properties {
    _Ratio ("Ratio", float) = 0.5
    _Color ("Base Color", Color) = (1,1,0)
    _InColor ("In Color", Color) = (1,0,0)
    _GridColor("Grid Color", Color) = (1,1,1)
    _GridWidth("Grid Width Rate", float) = 0.01
    _GridHeight("Grid Height Rate", float) = 0.01
    _Tex ("Base (RGB)", 2D) = "white" {}
  }
  SubShader {
  Tags {  "RenderType" = "Opaque" "Queue" = "Transparent" }
  Cull Off
  ZWrite Off
  Lighting Off

  CGPROGRAM
  #pragma surface surf Lambert alpha
  struct Input {
      float2 uv_Tex;
  };
  float4 _Color;
  float4 _InColor;
  float4 _GridColor;
  float _Ratio;
  float _GridWidth;
  float _GridHeight;	
  sampler2D _Tex;
  void surf (Input IN, inout SurfaceOutput o) {
    bool isEdgeMin = (min(IN.uv_Tex.x-_GridWidth,IN.uv_Tex.y-_GridHeight)<0);
    bool isEdgeMax = (max(IN.uv_Tex.x+_GridWidth,IN.uv_Tex.y+_GridHeight)>1);
    float4 col = (IN.uv_Tex.x<_Ratio ? _InColor:_Color)*tex2D(_Tex, IN.uv_Tex); 
    o.Emission = (isEdgeMin||isEdgeMax) ? _GridColor.rgb : col.rgb ;
    o.Alpha = (isEdgeMin||isEdgeMax) ? _GridColor.a : col.a;
  }
  ENDCG
  } 
  FallBack "Diffuse"
}
