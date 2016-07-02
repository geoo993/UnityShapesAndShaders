Shader ".ShaderExample/NewColor"
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
				float2 textureCoordinate : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 textureCoordinate : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _Color;
			
			v2f vert (appdata IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.textureCoordinate = IN.textureCoordinate;//TRANSFORM_TEX(IN.uv, _MainTex);

				return OUT;
			}

			fixed4 frag(v2f IN) : COLOR
			{
				fixed4 textureColor = tex2D(_MainTex, IN.textureCoordinate);
				return textureColor;
				//return _Color;
			}
			ENDCG


		}



	}



}
