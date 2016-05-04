// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader ".ShaderExample/Diffuse"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "white" {}
	}
	SubShader
	{
//		Tags { "RenderType"="Opaque" }
//		LOD 100
//
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag


			// make fog work
			//#pragma multi_compile_fog
			
			//#include "UnityCG.cginc"

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

		
			sampler2D _MainTex;
			float4 _Color;
			float4 _LightColor0;

			v2f vert (appdata IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.normal = mul(float4(IN.normal,0.0), unity_ObjectToWorld).xyz;
				OUT.textureCoordinate = IN.textureCoordinate;//TRANSFORM_TEX(IN.uv, _MainTex);

				return OUT;
			}

			fixed4 frag(v2f IN) : COLOR
			{
				fixed4 textureColor = tex2D(_MainTex, IN.textureCoordinate);
				//return textureColor;

				float3 normalDirection = normalize(IN.normal);
				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				float3 diffuse = _LightColor0.rgb * max(0.0,dot(normalDirection, lightDirection));

				return _Color * textureColor * float4(diffuse,1);
				//return _Color;
			}
			ENDCG


		}



	}



}
