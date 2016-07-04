Shader "Oberonscourt/Overlay"
{
	Properties 
	{
_Color("_Color", Color) = (1,1,1,1)

	}
	
	SubShader 
	{
		Tags
		{
"Queue"="Overlay"
"IgnoreProjector"="False"
"RenderType"="Overlay"

		}

		
Cull Back
ZWrite On
ZTest Always
ColorMask RGBA
Fog{
}


	}
	Fallback "Diffuse"
}