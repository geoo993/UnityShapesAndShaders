// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader ".ShaderExample/PerlinNoise"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "white" {}
		_MainTex2("Main Texture", 2D) = "white" {}

		_Distort ("Distortion [0-100]", range (0, 100)) = 10
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag


			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 textureCoordinate : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float2 textureCoordinate : TEXCOORD0;
			};

		
			uniform float4 _MainTex_ST; 
			uniform float4 _MainTex2_ST; 
			sampler2D _MainTex;
			sampler2D _MainTex2;
			float4 _Color;
			float4 _LightColor0;
			float _Distort;

			v2f vert (appdata IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.normal = mul(float4(IN.normal,0.0), unity_ObjectToWorld).xyz;
				OUT.textureCoordinate = IN.textureCoordinate;//TRANSFORM_TEX(IN.uv, _MainTex);

				return OUT;
			}

//			fixed4 frag(v2f IN) : COLOR
//			{
//				fixed4 textureColor = tex2D(_MainTex, IN.textureCoordinate);
//				//return textureColor;
//
//				float3 normalDirection = normalize(IN.normal);
//				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
//				float3 diffuse = _LightColor0.rgb * max(0.0,dot(normalDirection, lightDirection));
//
//				return _Color * textureColor * float4(diffuse,1);
//				//return _Color;
//			}
//
//


			float4 frag(v2f IN) : COLOR{

		
			    //Get the appropriate color value from the first texture
			    half4 col = tex2D(_MainTex,IN.textureCoordinate.xy * _MainTex_ST.xy + _MainTex_ST.zw);

			    //Offset the coordinates for the second color value
			    //Then get the color value from the second texture
			    float2 offset = float2(_Distort * (col.r - 0.5), _Distort * (col.g - 0.5));

			    half4 col2 = tex2D(_MainTex2, IN.textureCoordinate.xy * _MainTex2_ST.xy + _MainTex2_ST.zw + offset);

			    //Return the grayscale value in the red channel
			    return col2.a;

			}

			ENDCG

		}


	}
	FallBack "Diffuse"



}
