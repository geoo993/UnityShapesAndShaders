Shader ".ShaderExample/PerlinVertexExtrudeLerp"
{
    Properties {
   
     	_MainTex ("Texture", 2D) = "white" {}
     	_Amount ("Extrusion Amount", Range(-1,1)) = 0.5

     	_Scale ("Scale", Vector) = (1, 1, 1)
		_Offset ("Offset", Vector) = (0, 0, 0)
		_Speed ("Speed", Vector) = (0, 0, 0)

    }
    SubShader 
    {
	    Tags { "RenderType" = "Opaque" }
	    CGPROGRAM
	    #pragma target 3.0	
	    #pragma surface surf Lambert vertex:vert

      	int perm(int d)
		{
			d = d % 256;
			float2 t = float2(d%16,d/16)/15.0;
			return t * 255;
		}

		float fade(float t) { return t * t * t * (t * (t * 6.0 - 15.0) + 10.0); }

		float lerp(float t,float a,float b) { return a + t * (b - a); }

		float grad(int hash,float x,float y,float z)
		{
			int h	= hash % 16;										// & 15;
			float u = h<8 ? x : y;
			float v = h<4 ? y : (h==12||h==14 ? x : z);
			return ((h%2) == 0 ? u : -u) + (((h/2)%2) == 0 ? v : -v); 	// h&1, h&2 
		}

		float noise(float x, float y,float z)
		{	
			int X = (int)floor(x) % 256;	// & 255;
			int Y = (int)floor(y) % 256;	// & 255;
			int Z = (int)floor(z) % 256;	// & 255;
			
			x -= floor(x);
			y -= floor(y);
			z -= floor(z);
		      
			float u = fade(x);
			float v = fade(y);
			float w = fade(z);
			
			int A	= perm(X  	)+Y;
			int AA	= perm(A	)+Z;
			int AB	= perm(A+1	)+Z; 
			int B	= perm(X+1	)+Y;
			int BA	= perm(B	)+Z;
			int BB	= perm(B+1	)+Z;
				
			return lerp(w, lerp(v, lerp(u, grad(perm(AA  ), x  , y  , z   ),
		                                   grad(perm(BA  ), x-1, y  , z   )),
		                           lerp(u, grad(perm(AB  ), x  , y-1, z   ),
		                                   grad(perm(BB  ), x-1, y-1, z   ))),
		                   lerp(v, lerp(u, grad(perm(AA+1), x  , y  , z-1 ),
		                                   grad(perm(BA+1), x-1, y  , z-1 )),
		                           lerp(u, grad(perm(AB+1), x  , y-1, z-1 ),
		                                   grad(perm(BB+1), x-1, y-1, z-1 ))));
		}

	    sampler2D _MainTex;
	    float3 _Scale, _Offset, _Speed;
	    float _Amount;

	    struct Input 
	    {
	        float2 uv_MainTex;
	        float timeR;
	        float timeG;
	        float timeB;
	    };

	    void vert (inout appdata_full IN) 
	    {
	    	float noiseX = IN.vertex.x * _Scale.x + _Offset.x + _Time * _Speed.x;
	    	float noiseY = IN.vertex.y * _Scale.y + _Offset.y + _Time * _Speed.y;
	    	float noiseZ = IN.vertex.z * _Scale.z + _Offset.z + _Time * _Speed.z;
	        IN.vertex.xyz += IN.normal * _Amount * noise(noiseX, noiseY, noiseZ);
	    }
	     
	    void surf (Input IN, inout SurfaceOutput o) {
	
			///(t/8, t/4, t/2, t)
			///cos(0.125, 0.25, 0.5, t)
			///sin(0.125, 0.25, 0.5, t)
			IN.timeR = sin( _Time );
			IN.timeG = cos( _Time );
			IN.timeB = cos( _Time );
	    	float4 res = float4( IN.timeR, IN.timeG , IN.timeB, 1.0);

	        o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb * res;
	    }
	    ENDCG

    } 
    Fallback "Diffuse"
 }