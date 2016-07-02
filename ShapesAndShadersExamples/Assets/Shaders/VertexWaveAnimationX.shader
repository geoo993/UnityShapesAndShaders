
Shader ".ShaderExample/VertexWaveAnimationX" {

	Properties {
   
		_Frequency ("Frequency", Range(-2.0,2.0)) = 0.5
		_Noise ("Noise", Range(-50.0,50.0)) = 0.0
		_Speed ("Speed", Range(0.0,20.0)) = 2.5

    
    }

  SubShader {

    Pass {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"


      float _Frequency, _Noise, _Speed;

      struct v2f {
          float4 pos : SV_POSITION;
          fixed4 color : COLOR;
      };
      
      v2f vert (appdata_base IN)
      {
          v2f OUT;

          // waving mesh on xxx axis
          IN.vertex.y += sin((IN.vertex.x + _Time * _Speed) * _Noise ) * _Frequency;

          OUT.color = float4(IN.normal,1) * 0.5 + 0.5;

          // this line must be after the vertex manipulation
          OUT.pos = mul (UNITY_MATRIX_MVP, IN.vertex);

          return OUT;
      }

      fixed4 frag (v2f i) : SV_Target 
      { 
      	return i.color;
      }
      ENDCG
    }


  } 



}
