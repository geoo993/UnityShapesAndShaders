Shader ".ShaderExample/DiffuseWithAmbient"
{
	Properties
	{
		[NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
		_Color ("Main Color", Color) = (1,1,1,1) 
	}
	SubShader
	{
		
		Pass
		{
			Tags {"LightMode"="ForwardBase"}
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

            //Needed for fog variation to be compiled.
            #pragma multi_compile_fog


           
			struct v2f
			{
				float2 uv : TEXCOORD0;
				fixed4 diffuse : COLOR0;
				float4 pos : SV_POSITION;

				//Used to pass fog amount around number should be a free texcoord.
                UNITY_FOG_COORDS(1)
			};

		



			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
		
			v2f vert (appdata_base IN)
            {
                v2f OUT;
                OUT.pos = mul(UNITY_MATRIX_MVP, IN.vertex);
                //OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.uv = TRANSFORM_TEX (IN.texcoord, _MainTex);
                //OUT.uv = IN.texcoord;


                half3 worldNormal = UnityObjectToWorldNormal(IN.normal);
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                OUT.diffuse = nl * _LightColor0;

                // the only difference from previous shader:
                // in addition to the diffuse lighting from the main light,
                // add illumination from ambient or light probes
                // ShadeSH9 function from UnityCG.cginc evaluates it,
                // using world space normal
                OUT.diffuse.rgb += ShadeSH9(half4(worldNormal,1));

                //Compute fog amount from clip space position.
                UNITY_TRANSFER_FOG(OUT,OUT.pos);

                return OUT;
            }

        

			fixed4 frag (v2f IN) : SV_Target
			{
				 
				// sample the texture
				fixed4 col = tex2D(_MainTex, IN.uv);
                col *= IN.diffuse;


               //fixed4 col = fixed4(IN.uv.xy,0,0);
                
                //Apply fog (additive pass are automatically handled)
                UNITY_APPLY_FOG(i.fogCoord, col); 
                
                //to handle custom fog color another option would have been 
                //#ifdef UNITY_PASS_FORWARDADD
                //  UNITY_APPLY_FOG_COLOR(i.fogCoord, col, float4(0,0,0,0));
                //#else
                //  fixed4 myCustomColor = fixed4(0,0,1,0);
                 // UNITY_APPLY_FOG_COLOR(i.fogCoord, col, _Color);
                //#endif


                return col;// * _Color;
            }
			ENDCG
		}
	}
}
