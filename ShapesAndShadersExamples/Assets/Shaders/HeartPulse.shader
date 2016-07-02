Shader ".ShaderExample/HeartPulse" 
{
	   Properties {
	       _Color ("Main Color", Color) = (0.5, 0.5, 0.5, 1)
	       _SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	       _Shininess ("Shininess", Range (0.01, 1)) = 0.078125
	       _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	       _BumpMap ("Normalmap", 2D) = "bump" {}
	       _myTime ("Time", Float) = 0.5 
	       _xOffSet ("X", Range (-50, 50)) = 0
	       _yOffSet ("Y", Range (-50, 50)) = 0
	       _zOffSet ("Z", Range (-50, 50)) = 0
	   }

	   SubShader {
	   Tags {"Queue"="Transparent" 
	        "IgnoreProjector"="True" 
	        "RenderType"="Transparent"
	   }
	   LOD 400
	   CGPROGRAM
	   #pragma surface surf BlinnPhong alpha vertex:vert
	   // vertex:vert => there's a vertex shader and it's named vert
	   #include "UnityCG.cginc"

	   sampler2D _MainTex;
	   sampler2D _BumpMap;
	   sampler2D _LightMap;
	   float4 _Color;
	   float _Shininess;
	   int _xOffSet, _yOffSet, _zOffSet;

	   struct Input {
	       float2 uv_MainTex;
	       float2 uv_BumpMap;
	       float4 color : COLOR;
	   };

	   // our vertex shader, that's where the magic happens!
	   void vert (inout appdata_full IN) 
	   {
	      // v.color is storing the vertex color of each vertex
	      // we define a phase, a timing for the animation which is
	      // the time since the start of the application (_Time[1])
	      // plus a delay proportional to the red channel of the vcolor
	      float phase = cos( _Time[1] * 5 + IN.color.r * 1) ;

	      // we offset the x position of the vertex currently processed
	      // by a value proportional to the blue channel of the vcolor
	      IN.vertex.x += _xOffSet * (IN.color.b - 0.5f) * phase * phase * phase * phase;

	      // we offset the y position of the vertex currently processed
	      // by a value proportional to the green channel of the vcolor
	      IN.vertex.y += _yOffSet * (IN.color.g - 0.5f) * phase * phase * phase * phase;


	       // we offset the z position of the vertex currently processed
	      // by a value proportional to the green channel of the vcolor
	      IN.vertex.z += _zOffSet * (IN.color.r - 0.5f) * phase * phase * phase * phase;
	   }

	   // a surface shader (very similar to the previous example)
	   void surf (Input IN, inout SurfaceOutput OUT) {
	       half4 tex = tex2D(_MainTex, IN.uv_MainTex);
	       half4 c = tex * _Color * 2;
	       OUT.Albedo = c.rgb ;
	       OUT.Gloss = tex.a * c.b;
	       OUT.Alpha = c.a ;
	       OUT.Specular = _Shininess;
	       OUT.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	   }
	   ENDCG
	   }
}