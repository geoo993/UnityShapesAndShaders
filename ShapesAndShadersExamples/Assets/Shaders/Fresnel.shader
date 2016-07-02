// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader ".ShaderExample/Diffuse Bump Frisnel" { 

Properties{ 

_MainTex("Diffuse Texture" , 2D) = "white"{} 
_BumpTex("Normal Map" , 2D) = "bump"{} 
_RimColor("Rim Color" , Color) = (1,1,1,1) 
_RimPower("Rim Power" , Range(0.1,10)) = 3
}
SubShader {  
   Tags { "RenderType" = "Opaque" }  
   CGPROGRAM  
   #pragma surface surf Lambert  
   struct Input {  
       float2 uv_MainTex;  
       float3 worldRefl;
       float3 viewDir;
   };  
   sampler2D _MainTex;  
   samplerCUBE _Cube;
   float _RimPower;
   void surf (Input IN, inout SurfaceOutput o) {  
       o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb * 0.5;
       half rim = saturate(dot (normalize(IN.viewDir), o.Normal));
       o.Emission = texCUBE (_Cube, IN.worldRefl).rgb * pow(rim,_RimPower);
   }  
   ENDCG  
 }   

 }
//Shader ".ShaderExample/Fresnel"
//{
//	Properties
//	{
//		_Color ("Main Color", Color) = (1,1,1,1)
//		_MainTex("Main Texture", 2D) = "white" {}
//	}
//	SubShader
//	{
//		Tags { "RenderType"="Opaque" }
//
//		//Pass
//		//{
////			CGPROGRAM
////			#pragma vertex vert
////			#pragma fragment frag
////
////
////			// make fog work
////			//#pragma multi_compile_fog
////			
////			//#include "UnityCG.cginc"
////
////			struct appdata
////			{
////				float4 vertex : POSITION;
////				float3 normal : NORMAL;
////				float2 textureCoordinate : TEXCOORD0;
////			};
////
////			struct v2f
////			{
////				float4 vertex : SV_POSITION;
////				float3 normal : NORMAL;
////				float2 textureCoordinate : TEXCOORD0;
////			};
////
////		
////			sampler2D _MainTex;
////			float4 _Color;
////			float4 _LightColor0;
////
////			v2f vert (appdata IN)
////			{
////				v2f OUT;
////				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
////				OUT.normal = mul(float4(IN.normal,0.0), unity_ObjectToWorld).xyz;
////				OUT.textureCoordinate = IN.textureCoordinate;//TRANSFORM_TEX(IN.uv, _MainTex);
////
////				return OUT;
////			}
////
////			fixed4 frag(v2f IN) : COLOR
////			{
////				fixed4 textureColor = tex2D(_MainTex, IN.textureCoordinate);
////				//return textureColor;
////
////				float3 normalDirection = normalize(IN.normal);
////				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
////				float3 diffuse = _LightColor0.rgb * max(0.0,dot(normalDirection, lightDirection));
////
////				return _Color * textureColor * float4(diffuse,1);
////				//return _Color;
////			}
////			ENDCG
//
//		     CGPROGRAM
//		     #pragma surface surf Lambert
//		     
//		     struct Input
//		     {
//		         float2 uv_MainTex;
//		         float3 viewDir;
//		     };
//		     
//		     sampler2D _MainTex;
//		     sampler2D _BumpTex;
//		     float4 _RimColor;
//		     float _RimPower;
//		     
//		     void surf(Input IN,inout SurfaceOutput o){
//		         fixed4 tex = tex2D (_MainTex, IN.uv_MainTex);
//		         o.Albedo = tex.rgb;
//		         o.Normal =  UnpackNormal (tex2D (_BumpTex, IN.uv_MainTex));
//		         half rim = 1 - saturate(dot(normalize(IN.viewDir), o.Normal));
//		         o.Emission = _RimColor.rgb * pow(rim, _RimPower);
//		     }
//		     ENDCG
//
//		//}
//
//
//
//
//
//	}
//
//
//
//}
