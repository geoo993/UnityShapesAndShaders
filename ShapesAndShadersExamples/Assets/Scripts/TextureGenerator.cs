using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextureGenerator : MonoBehaviour {


	[Range(2, 512)] public int resolution = 256;

	MeshRenderer renderer;
	private Texture2D texture;
	private Color color = Color.blue;
	[Range(2, 256)] public float frequency = 16f;
	[Range(1, 3)] public int dimensions = 3;


	Color[] colors = new Color[] {
		Color.red, 
		Color.yellow, 
		Color.green, 
		Color.blue,
		Color.cyan,
		Color.gray,
		Color.magenta,
		Color.black,
		Color.white
	};


	List<Vector2[]> pixelsPoints = new List<Vector2[]>();
	List <Color> interpolateColorsA = new List<Color>();
	List <Color> interpolateColorsB = new List<Color>();
	List <Color> interpolateComplete = new List<Color>();

	List <float> id = new List<float>();
	List <float> times = new List<float>();



	//private void Awake () {
	private void OnEnable () {

		renderer = GetComponent<MeshRenderer> ();
		if (texture == null) {


			// new Texture2D (width , heigth, TextureFormat: format, bool : mipmap)
			texture = new Texture2D (resolution, resolution, TextureFormat.RGB24, false);
			//texture = new Texture2D(resolution, resolution, TextureFormat.ARGB32, false);
			texture.name = "Procedural Texture";

			texture.wrapMode = TextureWrapMode.Clamp;
			texture.filterMode = FilterMode.Trilinear;//FilterMode.Bilinear; //FilterMode.Point;
			texture.anisoLevel = 9;

			renderer.material.mainTexture = texture;
		}
	
			FillTexture ();
		
	}




	private void Update () {
		
		if (transform.hasChanged) {
			transform.hasChanged = false;
			FillTexture();
		}


//		//int r = 235;
//		for (int r = 0; r < resolution; r++) {
//
//			if (id[r] < 1.0f) {
//				id[r] += Time.deltaTime * (1.0f / times[r]);
//			} else {
//				id [r] = 0;
//				times[r] = Random.Range (10f, 20f);
//
//				interpolateColorsA[r] = interpolateComplete[r];
//				interpolateColorsB[r] = colors[Random.Range(0,colors.Length-1)];
//			}
//
//			interpolateComplete[r] = Color.Lerp (interpolateColorsA[r], interpolateColorsB[r], id[r]);
//		
//			//print ("i: "+r+"    "+pixelsPoints[0] [r]);
//			int xMin = ((int)pixelsPoints [r] [pixelsPoints [r].Length - 1].x -  (int)frequency);
//			int xMax = (int)pixelsPoints [r] [pixelsPoints [r].Length - 1].x;
//
//			int yMin = ((int)pixelsPoints [r] [pixelsPoints [r].Length - 1].y -  (int)frequency);  
//			int yMax = (int)pixelsPoints [r] [pixelsPoints [r].Length - 1].y;
//
//			for (int u = yMin; u < yMax; u++) {
//
//				for (int h = xMin; h < xMax; h++) {
//
//				//print (texture.GetPixel (h + 1, u + 1));
//					texture.SetPixel (h, u, interpolateComplete[r]);
//				}
//			}
//				
//		}

		//print ("idd  "+id [20]+"   times: "+times[20]);
		//print (pixalColors.Count + "   " + pixelsPoints.Count);

		// Apply all SetPixel calls
		texture.Apply();

	}


	public void FillTexture () {

		//the local coordinates of a quad with center (0,0)

		Vector3 point00 = transform.TransformPoint(new Vector3(-0.5f,-0.5f, 0f));
		Vector3 point10 = transform.TransformPoint(new Vector3( 0.5f,-0.5f, 0f));
		Vector3 point01 = transform.TransformPoint(new Vector3(-0.5f, 0.5f, 0f));
		Vector3 point11 = transform.TransformPoint(new Vector3( 0.5f, 0.5f, 0f));

		//// the texture coordinated map values 
//		texture.SetPixel(0, 0, new Color(1,1,1,1));
//		texture.SetPixel(1, 0, Color.clear);
//		texture.SetPixel(0, 1, Color.white);
//		texture.SetPixel(1, 1, Color.black);
//
	

		if (texture.width != resolution) {
			texture.Resize(resolution, resolution);
		}

		Color randColor = new Color(Random.value, Random.value, Random.value, 1f);


		NoiseMethod method = PseudorandomNoise.valueMethods[dimensions - 1];
		float stepSize = 1f / resolution;
		//Random.seed = 42;


		for (int y = 0; y < resolution; y++) {

			////interpolate between the bottom left and top left corner based on y
			Vector3 point0 = Vector3.Lerp(point00, point01, (y + 0.5f) * stepSize);

			////interpolate between the bottom right and top right corner based on x
			Vector3 point1 = Vector3.Lerp(point10, point11, (y + 0.5f) * stepSize);	


			for (int x = 0; x < resolution; x++) {

				////use bilinear interpolation to find the final point, which we directly convert into a color.
				Vector3 point = Vector3.Lerp(point0, point1, (x + 0.5f) * stepSize);

				//color = ((x & y) != 0 ? Color.white : Color.gray);  //// pattern 0
				//color = new Color(x * stepSize, y * stepSize, 0f);   //// pattern 1
				//color = new Color((x + 0.5f) * stepSize, (y + 0.5f) * stepSize, 0f);   //// pattern 2
				//color = new Color((x + 0.5f) * stepSize % 0.1f, (y + 0.5f) * stepSize % 0.1f, 0f) * 10f;  //// pattern 3 //// Repeating the pattern.
				Vector3 pattern2 = new Vector3((x + 0.5f) * stepSize, (y + 0.5f) * stepSize, 0f);

				//color = new Color(point.x, point.y, point.z);

				//color = Color.white * Random.value; ////  noise
				color = Color.white * method(point,frequency,true); //// pseudorandom noise

				////each pixel and color given
				texture.SetPixel(x, y, color);


			}
		}




//		int v = 0;
//		for (int x = 0; x < (int)frequency; x++) {
//
//			List<Vector2> innerArray = new List<Vector2>();
//
//			for (int y = 0; y < (int)frequency; y++) {
//
//
//				Color c = ExtensionMethods.RandomColor ();
//				int yOffset1 = (int)frequency * x;
//				int yOffset2 = (resolution - (resolution - ((int)frequency * x))) + (int)frequency; 
//
//				for (int yOff = yOffset1; yOff < yOffset2; yOff++) {
//
//					int xOffset1 = (int)frequency * y;
//					int xOffset2 = (resolution - (resolution - ((int)frequency * y))) + (int)frequency; 
//					for (int xOff = xOffset1 ; xOff < xOffset2; xOff++) {
//
//						texture.SetPixel (xOff, yOff, c);
//						innerArray.Add (new Vector2(xOff, yOff));
//					}
//
//				}
//
//				pixelsPoints.Insert(v, innerArray.ToArray ());
//
//				interpolateColorsA.Add (Color.red);
//				interpolateColorsB.Add (Color.green);
//				interpolateComplete.Add (Color.black);
//
//				id.Add (0.0f);
//				times.Add (Random.Range (10f, 20f));
//
//				v++;
//				//print (v);
//			}
//
//		}
		//				///////CHANGE INDIVIDUAL BLOCKS COLOR
		//				///////INTERPOLATE BETWEEN THE COLOR 

		//print (pixelsPoints.Count);




//		//int r = 235;
//		for (int r = 0; r < resolution; r++) {
//
//			Color startC = ExtensionMethods.RandomColor ();
//			Color endC = ExtensionMethods.RandomColor ();
//			Color c = Color.Lerp(startC, endC, timer/100);
//
//			//print ("i: "+r+"    "+pixelsPoints[0] [r]);
//			int xMin = ((int)pixelsPoints [r] [pixelsPoints [r].Length - 1].x -  (int)frequency);
//			int xMax = (int)pixelsPoints [r] [pixelsPoints [r].Length - 1].x;
//
//			int yMin = ((int)pixelsPoints [r] [pixelsPoints [r].Length - 1].y -  (int)frequency);  
//			int yMax = (int)pixelsPoints [r] [pixelsPoints [r].Length - 1].y;
//
//			for (int u = yMin; u < yMax; u++) {
//				
//				for (int h = xMin; h < xMax; h++) {
//
//					texture.SetPixel (h, u, c);
//
//				}
//			}
//		}



		// Apply all SetPixel calls
		texture.Apply();
	}
		
}

