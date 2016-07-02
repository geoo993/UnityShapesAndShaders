Shader ".ShaderExample/ObjectNormalsAsColours"
{
	Properties
	{
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

	        struct v2f {
                // we'll output world space normal as one of regular ("texcoord") interpolators
                half3 worldNormal : TEXCOORD0;
                float4 pos : SV_POSITION;
                fixed3 color : COLOR0;
            };
           //  vertex shader: takes object space normal as input too
            v2f vert (float4 vertex : POSITION, float3 normal : NORMAL)
            {
                v2f OUT;
                OUT.pos = mul(UNITY_MATRIX_MVP, vertex);
                // UnityCG.cginc file contains function to transform
                // normal from object to world space, use that
                OUT.worldNormal = UnityObjectToWorldNormal(normal);
                OUT.color = normal * 0.5 + 0.5;
                return OUT;
            }


 			fixed4 frag (v2f IN) : SV_Target
            {
                fixed4 normalColor = 0;
                // normal is a 3D vector with xyz components; in -1..1
                // range. To display it as color, bring the range into 0..1
                // and put into red, green, blue components
                normalColor.rgb = IN.worldNormal * 0.5 + 0.5;

                fixed4 normalCol2 = fixed4 (IN.color, 1);
                return normalColor * normalCol2; //* _MyColor; 
            }
            ENDCG


		}
	}
}
