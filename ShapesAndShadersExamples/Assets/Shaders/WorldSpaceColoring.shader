Shader ".ShaderExample/WorldSpaceColoring" 
 {
     Properties 
     {
         _MaskTex ("_MaskTex", 2D) = "white" {}
         _MainColor ("_MainColor", Color) = (1,1,1,1)
         _NeighbourPos("_NeighbourPos", Vector) = (1,1,1,1)
 
     }
     SubShader
     {
         Lighting Off 
         Fog { Mode Off }
         ZWrite Off
         Blend SrcAlpha OneMinusSrcAlpha      
 
 
         Tags { "Queue"="Transparent" "RenderType"="Transparent" }
         Pass
         {
             CGPROGRAM
 
             #pragma vertex vert
             #pragma fragment frag
 
             #include "UnityCG.cginc"
             
             sampler2D _MaskTex;
             uniform fixed4 _MainColor;
             uniform float _Scale;
             uniform float4 _NeighbourPos;
 
             struct vertOut 
             {
                 float4 pos : SV_POSITION;
                 float2 tex : TEXCOORD0;
                 float3 lpos : TEXCOORD2;
             };
             
 
             vertOut vert(appdata_full input)
             {
                 vertOut output;
 
                 
                 fixed4 pos = input.vertex; 
 
                 output.lpos = input.vertex;
                 output.pos = mul (UNITY_MATRIX_MVP, pos);
                 output.tex = input.texcoord;
                 return output;
             }
 
             fixed4 frag(vertOut input) : COLOR0
             {
                 fixed4 output = fixed4(0,0,0,0);
                 output.a = 1;
                 output.rgb = input.lpos;
 
                 return output;
             }
 
             ENDCG
         }
     }
 }