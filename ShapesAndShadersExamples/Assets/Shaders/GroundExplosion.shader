// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader ".ShaderExample/GroundExplosion"
{

	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	 
		_Color ("Color", Color) = (1,1,1,1)
		 
		_ExtrudeTex ("Extrusion Texture", 2D) = "white" {}
		_Amount ("Extrusion Multiplier", Range(0,5)) = 0
		_Pos ("Pos", Vector) = (0,0,0,0)
	}
 
	SubShader
	{
	//    Pass {
	//        Cull Off
	//        ZWrite On
	//        ZTest Always
 
		Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Lambert vertex:vert
		#include "UnityCG.cginc"
		 
	 
		struct Input 
		{
			float2 vpos;
			float2 uv_MainTex;
		};
 
 
		float4 _Color;
		float4 _Pos;
		float _Amount;
		sampler2D _ExtrudeTex;
		float4  _ExtrudeTex_ST;

 	

		void vert (inout appdata_full v, out Input o)
		{
	 
			float t = _Time.y * 2;
			 
			#if !defined(SHADER_API_OPENGL)
			float tex = tex2Dlod(_ExtrudeTex, float4(float2(v.texcoord.x,v.texcoord.y),0.0,0.0)).r;
			float3 worldPos = mul (unity_ObjectToWorld, v.vertex).xyz;
			float dist = distance(worldPos,_Pos.xyz);
			float3 dir = -normalize(_Pos.xyz-worldPos) * 0.25f;
			 
	 
			if (t>dist)
			{
				float3 extrude = (v.normal+dir)*tex.r* sqrt(_Amount * (t-dist));
				v.vertex.xyz += extrude;
			}
			 
			o.vpos = v.vertex.xy;


			#endif
		}
 
		sampler2D _MainTex;
		 
		 
		void surf (Input IN, inout SurfaceOutput o)
		{
			half4 tex1 = tex2D (_MainTex, IN.uv_MainTex);
			half4 tex2 = tex2D (_MainTex, IN.uv_MainTex*5-_Time.yy*0.5f)+_Color;
			half4 col = lerp(tex1,tex2,IN.vpos.y);
			o.Albedo = col;//*(1-_Color*IN.vpos.y);
		}
	 
		ENDCG
	}
	Fallback "Diffuse"
}

