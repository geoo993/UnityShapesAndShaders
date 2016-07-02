Shader ".ShaderExample/Glow"
{
	Properties
	{
		_ColorTint("Color Tint", Color) = (1, 1, 1, 1)

		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BumpMap("Normal Map", 2D) = "bump" {}

		_RimColor ("Rim Color", Color) = (1, 1, 1, 1)
      	_RimPower ("Rim Power", Range(0.5,8.0)) = 3.0

    	_SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
    	_Shininess ("Shininess", Range (0.01, 1)) = 0.7

	}


	SubShader
	{
		//Tags { "RenderType"="Opaque" }
		Tags { "RenderType" = "Transparent" "Opaque" = "Transparent" }  

			CGPROGRAM
			#pragma surface surf SimpleSpecular alpha 
			//#pragma surface surf Lambert    
		

			sampler2D _MainTex;
			float _Shininess;
			float4 _ColorTint;
			sampler2D _BumpMap;
			float4 _RimColor;
			float _RimPower;


			half4 LightingSimpleSpecular (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
		   {

		   		half3 h = normalize (lightDir + viewDir);
		   		half diff = max ( 0, dot (s.Normal, lightDir));
		   		float nh = max ( 0, dot (s.Normal, h));
		   		float spec = pow (nh, 48.0);
		   		half4 c;
		   		c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec * s.Alpha * _Shininess * _SpecColor) * (atten * 2);
		   		c.a = s.Alpha;
		   		return c;
		   }



			struct Input {
		   		float2 uv_MainTex;
		   		float4 color : Color;
		   		float2 uv_BumpMap;
		   		float3 viewDir;
		   	};


		   	void surf ( Input IN, inout SurfaceOutput OUT) {

		   		IN.color = _ColorTint;
		   		fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
		   		OUT.Albedo = c.rgb * IN.color;
		   		OUT.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));

		   		half rim = 1.0 - saturate ( dot (normalize( IN.viewDir), OUT.Normal));
		   		OUT.Emission = _RimColor.rgb * pow (rim, _RimPower);
		   		OUT.Alpha = c.a + rim;

		   }


			ENDCG


	}


	Fallback "Diffuse"
}
