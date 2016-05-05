// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader ".ShaderExample/SkyReflection"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,0.5) 

		 // three textures we'll use in the material
        _MainTex("Base texture", 2D) = "white" {}
        _OcclusionMap("Occlusion", 2D) = "white" {}
        _BumpMap("Normal Map", 2D) = "bump" {}

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


            // exactly the same as in previous shader
            struct v2f {
                float3 worldPos : TEXCOORD0;
                half3 tspace0 : TEXCOORD1;
                half3 tspace1 : TEXCOORD2;
                half3 tspace2 : TEXCOORD3;
                float2 worldReflectionUV : TEXCOORD4;
                float4 pos : SV_POSITION;
            };

			 // textures from shader properties
			fixed4 _Color; 
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _OcclusionMap;
            sampler2D _BumpMap;
        


        	v2f vert (float4 vertex : POSITION, float3 normal : NORMAL, float4 tangent : TANGENT, float2 uv : TEXCOORD0)
            {
                v2f OUT;
                OUT.pos = mul(UNITY_MATRIX_MVP, vertex);
                OUT.worldPos = mul(unity_ObjectToWorld, vertex).xyz;
                half3 wNormal = UnityObjectToWorldNormal(normal);
                half3 wTangent = UnityObjectToWorldDir(tangent.xyz);
                half tangentSign = tangent.w * unity_WorldTransformParams.w;
                half3 wBitangent = cross(wNormal, wTangent) * tangentSign;
                OUT.tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
                OUT.tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
                OUT.tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);
                OUT.worldReflectionUV = uv;
                return OUT;
            }

            fixed4 frag (v2f IN) : SV_Target
            {
                // same as from previous shader...
                half3 tnormal = UnpackNormal(tex2D(_BumpMap, IN.worldReflectionUV));
                half3 worldNormal;
                worldNormal.x = dot(IN.tspace0, tnormal);
                worldNormal.y = dot(IN.tspace1, tnormal);
                worldNormal.z = dot(IN.tspace2, tnormal);
                half3 worldViewDir = normalize(UnityWorldSpaceViewDir(IN.worldPos));
                half3 worldRefl = reflect(-worldViewDir, worldNormal);
                half4 skyData = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, worldRefl);
                half3 skyColor = DecodeHDR (skyData, unity_SpecCube0_HDR);                
                fixed4 c = 0;
                c.rgb = skyColor;

                // modulate sky color with the base texture, and the occlusion map
                fixed3 baseColor = tex2D(_MainTex, IN.worldReflectionUV).rgb;
                fixed occlusion = tex2D(_OcclusionMap, IN.worldReflectionUV).r;
                c.rgb *= baseColor;
                c.rgb *= occlusion;

                return c;
            }

			ENDCG
		}
	}
}
