Shader ".ShaderExample/Explosions/DrawPyroclastic Noises" 
{
//	Properties 
//	{
//		_GradientTex ("Base (RGB)", 2D) = "white" {}
//	}
//	SubShader 
//	{
//		Pass
//		{
//			Tags { "RenderType"="Transparent" }
//			LOD 200
//			Blend SrcAlpha OneMinusSrcAlpha
//
//			CGPROGRAM
//				#pragma target 5.0
//
//				#pragma debug
//				#pragma vertex vert
//				#pragma fragment frag
//		
//				#include "UnityCG.cginc" 
//				#include "SlowNoiseComputer.cginc" 
//
//				float4 _Position;
//				float4 _Rotation;
//				float _Scale;
//
//				float _Distortion;
//				float _Octaves;
//
//				float4x4 _Camera2World;
//
//				sampler2D _GradientTex;
//				float4 _GradientTex_ST;
//
//				struct v2f 
//				{
//					float4  pos : SV_POSITION;
//					float2  uv : TEXCOORD0;
//				};		
//
//
//				// **************************************************************
//				// Ray marching helpers											*
//				// **************************************************************
//
//				// Scene helpers-------------------------------------------------
//				float Union(float x, float y)
//				{
//					return min(x, y);
//				}
//
//				float Distort(float3 pos, float strength)
//				{
//					return fBm(pos, _Octaves) * strength;
//				}
//
//
//				// Shape helpers-------------------------------------------------
//				float DrawPyroclastic(float3 pos, float3 center, float size, float distortion)
//				{
//					float3 d = pos - center;
//					return length(d) - size - Distort(pos, distortion);
//				}
//				
//				float DrawSphere(float3 pos, float3 center, float radius)
//				{
//					float3 d = pos - center;
//					return length(d) - radius;
//				}
//
//
//
//				// Scene setup, draw shapes in here.
//				float RenderScene(float3 pos)
//				{	
//					float sceneDistance = DrawPyroclastic(pos, _Position.xyz, _Scale, _Distortion);
//					return sceneDistance;
//				}
//
//				// Basic ray marching method.
//				bool Trace(float3 rayOrigin, float3 rayDir, out float3 position, out float dist)
//				{
//					float distanceAccumulator = 0.0f;
//					position = float3(0, 0, 0);
//
//					float maxDistance = 1000.0f;
//					float minDistance = 0.0001f;
//					dist = 0.0f;
//
//					while (distanceAccumulator < maxDistance)
//					{
//						position = rayOrigin + distanceAccumulator * rayDir;
//
//						dist = RenderScene(position);
//
//						if (dist < minDistance)
//							break;
//		
//						distanceAccumulator += dist;
//					}
//
//					return (dist < minDistance);
//				}
//
//				// Lighting helpers--------------------------------------------------
//				float3 GetNormal(float3 pos)
//				{
//					float epsilon = 0.01;
//					float3 epsilonX = float3(epsilon, 0.0, 0.0);
//					float3 epsilonY = float3(0.0, epsilon, 0.0);
//					float3 epsilonZ = float3(0.0, 0.0, epsilon);
//
//					float3 deltaNormal;
//
//					deltaNormal.x = RenderScene(pos + epsilonX) - RenderScene(pos - epsilonX);
//					deltaNormal.y = RenderScene(pos + epsilonY) - RenderScene(pos - epsilonY);
//					deltaNormal.z = RenderScene(pos + epsilonZ) - RenderScene(pos - epsilonZ);
//					
//					return normalize(deltaNormal);
//				}
//
//				// Ambient occlusion.
//				float GetAO(float3 pos, float3 normal)
//				{
//					float multiplier = 4.0f;
//					float ao = 0.0f;
//					float decay = 1.0f;
//					int steps = 8;
//
//					for (int i = 0; i < steps; ++i)
//					{
//						float aoDist = multiplier * (float(i) / float(steps));
//						float3 aoPoint = pos + normal * (aoDist + 0.1f);
//						ao += decay * (aoDist - RenderScene(aoPoint));
//		
//						decay *= 0.5f;
//					}
//	
//					return clamp(1.0f - ao, 0.0f, 1.0f);
//				}
//
//				float GetShadow(float3 pos, float3 light)
//				{
//					float multiplier = 6.0f;
//					float shadow = 0.0f;
//					int steps = 10;
//
//					for (int i = 0; i < steps; ++i)
//					{
//						float shadowDist = multiplier * float(i) / float(steps);
//						float3 shadowPoint = pos  + light * shadowDist;
//						shadow += RenderScene(shadowPoint);
//					}
//	
//					return clamp(shadow, 0.0f, 1.0f);
//				}
//
//
//				// Materials---------------------------------------------------
//				float4 ApplyMaterial(float3 position, float3 rayOrigin, float3 rayDir, float dist)
//				{
//					// Do lighting (if any)
//					float3 normal = GetNormal(position);
//					float diffuseFactor = dot(normal, -rayDir);
//					float ao = GetAO(position, normal);
//
//					// Shade with displacement
//					float shade = dist - DrawSphere(position, _Position.xyz, _Scale); // Find difference between normal sphere and pyroclastic one to get shading.
//
//					return tex2D(_GradientTex, float2(1-shade *10, 0.5f)) * ao;	
//				}
//
//				float4 Background(float3 position, float3 rayDir)
//				{
//					return float4(0, 0, 0, 0);	
//				}
//
//
//
//				// **************************************************************
//				// Shader Programs												*
//				// **************************************************************
//
//				// Vertex Shader ------------------------------------------------
//				v2f vert (appdata_base v)
//				{
//					v2f o;
//
//					o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
//					o.uv = TRANSFORM_TEX (v.texcoord, _GradientTex);
//
//					return o;
//				}
//
//				
//				// Fragment Shader -----------------------------------------------
//				float4 frag(v2f i) : COLOR
//				{
//					float3 pixelWorldSpace = mul(_Camera2World, float4(-(i.uv.x-0.5f) * 2, -(i.uv.y-0.5f) * 2, 1.0f, 1.0f)).xyz;
//					float3 rayOrigin = _WorldSpaceCameraPos;
//					float3 rayDir = normalize(pixelWorldSpace);
//	
//					float3 position = float3(0, 0, 0);
//
//					float dist;
//					float4 output;
//
//					if (Trace(rayOrigin, rayDir, position, dist))
//						output = ApplyMaterial(position, rayOrigin, rayDir, dist);
//					else
//						output = Background(position, rayDir);
//
//					return float4(output.xyz, 1);
//				}
//
//			ENDCG	
//		}
//	}
}
