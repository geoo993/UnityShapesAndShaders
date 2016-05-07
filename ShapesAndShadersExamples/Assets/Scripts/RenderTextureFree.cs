using UnityEngine;
using System.Collections;

public class RenderTextureFree : MonoBehaviour {



	private static Texture2D tex2D;
	private static Texture tex;

	// Return the entire screen in a texture
	public static Texture Capture(){ return Capture( new Rect( 0f, 0f, Screen.width, Screen.height ), 0, 0 ); }

	// Return part of the screen in a texture.
	public static Texture Capture( Rect captureZone, int destX, int destY )
	{
		Texture2D result;
		result = new Texture2D( Mathf.RoundToInt( captureZone.width ) + destX,
			Mathf.RoundToInt( captureZone.height ) + destY,
			TextureFormat.RGB24, false);
		result.ReadPixels(captureZone, destX, destY, false);

		// That's the heavy part, it takes a lot of time.
		result.Apply();

		return result;
	}


}
