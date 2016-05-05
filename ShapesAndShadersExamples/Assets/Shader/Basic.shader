Shader ".ShaderExample/Basic"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MyColor ("Main Color", Color) = (1,1,1,1) 
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			fixed4 _MyColor; // low precision type is usually enough for colors
			sampler2D _MyTexture;

			struct v2f {
                // we'll output world space normal as one of regular ("texcoord") interpolators
                float2 worldNormal : TEXCOORD0;
                float4 pos : SV_POSITION;
                fixed3 color : COLOR0;
            };

            float4 _MyTexture_ST;

            v2f vert (appdata_base IN)
	       	{
	            v2f OUT;
	            OUT.pos = mul (UNITY_MATRIX_MVP, IN.vertex);
	            OUT.worldNormal = TRANSFORM_TEX (IN.texcoord, _MyTexture);
	            OUT.color = IN.normal * 0.5 + 0.5;
	            return OUT;
	        }


	        fixed4 frag (v2f IN) : SV_Target
	        {
	            fixed4 texcol = tex2D (_MyTexture, IN.worldNormal);
	            fixed4 normalCol = fixed4 (IN.color, 1);
	            return texcol * _MyColor; //* normalCol;
	        }
	        ENDCG

		}
	}
}
