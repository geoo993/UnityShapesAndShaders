using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextureGenerator : MonoBehaviour {


	[Range(2, 512)] public int resolution = 256;

	MeshRenderer renderer;
	private Texture2D texture;
	private Color color = Color.blue;

	[Range(2, 36)] public float frequency = 16f;
	[Range(1, 8)] public int octaves = 1;
	[Range(1f, 4f)] public float lacunarity = 2f;
	[Range(0f, 1f)] public float persistence = 0.5f;
	[Range(1, 3)] public int dimensions = 3;
	public Gradient gradientColor = new Gradient();
	public bool noiseColor = false;

	public bool fractal = true;
	public bool noiseInterpolation = false;
	public enum NoiseMethodType { Pseudo, Perlin };
	public NoiseMethodType noiseType = NoiseMethodType.Pseudo;
	NoiseMethod method;

	public enum TextureType { Noise, Stripes };
	public TextureType textureType = TextureType.Noise;


	List<Vector2[]> pixelsPoints = new List<Vector2[]>();
	List <Color> interpolateColorsA = new List<Color>();
	List <Color> interpolateColorsB = new List<Color>();
	List <Color> interpolateComplete = new List<Color>();

	Color[] colors = new Color[]
	{
		//Color.red, 
		Color.yellow, 
		Color.green, 
		//Color.blue,
		Color.cyan,
		//Color.gray,
		//Color.magenta,
		//Color.black,
		Color.white
	};
		
	List <float> id = new List<float>();
	List <float> times = new List<float>();

	public bool stripesFrequency = false;
	private float stripeFrequency = 16;//32
	private int stripeLoop = 16;
	private int stripeResolution = 256;


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
	
			//FillTexture ();
		
	}
	private void Update () {

		if (transform.hasChanged) {
			transform.hasChanged = false;
			FillTexture();
		}


		UpdateStripesColor ();


	}

	private void addGradient (Gradient g)
	{

		GradientColorKey blue = new GradientColorKey(Color.blue, 0.0f);
		GradientColorKey white = new GradientColorKey(Color.white, 0.3f);
		GradientColorKey black = new GradientColorKey(Color.black, 0.45f);
		GradientColorKey yellow = new GradientColorKey(Color.yellow, 0.6f);
		GradientColorKey red = new GradientColorKey(Color.red, 1f);

		GradientAlphaKey blueAlpha = new GradientAlphaKey(1,0);
		GradientAlphaKey yellowAlpha = new GradientAlphaKey(1,1);


		GradientColorKey[] colorKeys = new GradientColorKey[]{blue, white, black, yellow, red};
		GradientAlphaKey[] alphaKeys = new GradientAlphaKey[]{blueAlpha,yellowAlpha};
		g.SetKeys(colorKeys, alphaKeys);


	}



	public static float Sum (NoiseMethod method, Vector3 point, float frequency, int octaves, float lacunarity, float persistence, bool interpolate) {

		////this is a summation of noise frequencies which is known as 1/f noise, or fractal noise, or fractional noise, pink noise, and some other names
		float sum = method(point, frequency,interpolate);
//		float fractalSum = method (point * 2f, frequency * 2f, interpolate);
//		return ((sum + fractalSum) * 0.5f) / 1.5f;
//


		float amplitude = 1f;
		float range = 1f;
		for (int o = 1; o < octaves; o++) {
			frequency *= lacunarity;
			amplitude *= persistence;
			range += amplitude;
			sum += method(point, frequency,interpolate) * amplitude;
		}
		return sum / range;

	}

	public void FillTexture () {


		if (texture.width != resolution) {
			texture.Resize(resolution, resolution);
		}


		switch (textureType) {

		case TextureType.Noise:

			renderer.material.mainTexture = null;

			texture = new Texture2D (resolution, resolution, TextureFormat.RGB24, false);
			texture.name = "Noise Texture";

			texture.wrapMode = TextureWrapMode.Clamp;
			texture.filterMode = FilterMode.Trilinear;//FilterMode.Bilinear; //FilterMode.Point;
			texture.anisoLevel = 9;

			if (noiseType == NoiseMethodType.Pseudo) {
				method = PseudorandomNoise.valueMethods [dimensions - 1];
			} else if (noiseType == NoiseMethodType.Perlin) {
				method = PerlinNoise.perlinMethods [dimensions - 1];
			}


			//the local coordinates of a quad with center (0,0)
			Vector3 point00 = transform.TransformPoint (new Vector3 (-0.5f, -0.5f, 0f));
			Vector3 point10 = transform.TransformPoint (new Vector3 (0.5f, -0.5f, 0f));
			Vector3 point01 = transform.TransformPoint (new Vector3 (-0.5f, 0.5f, 0f));
			Vector3 point11 = transform.TransformPoint (new Vector3 (0.5f, 0.5f, 0f));

			float stepSize = 1f / resolution;
			//Random.seed = 42;

			for (int y = 0; y < resolution; y++) {

				////interpolate between the bottom left and top left corner based on y
				Vector3 point0 = Vector3.Lerp (point00, point01, (y + 0.5f) * stepSize);

				////interpolate between the bottom right and top right corner based on x
				Vector3 point1 = Vector3.Lerp (point10, point11, (y + 0.5f) * stepSize);	


				for (int x = 0; x < resolution; x++) {

					////use bilinear interpolation to find the final point, which we directly convert into a color.
					Vector3 point = Vector3.Lerp (point0, point1, (x + 0.5f) * stepSize);

					//color = ((x & y) != 0 ? Color.white : Color.gray);  //// pattern 0
					//color = new Color(x * stepSize, y * stepSize, 0f);   //// pattern 1
					//color = new Color((x + 0.5f) * stepSize, (y + 0.5f) * stepSize, 0f);   //// pattern 2
					//color = new Color((x + 0.5f) * stepSize % 0.1f, (y + 0.5f) * stepSize % 0.1f, 0f) * 10f;  //// pattern 3 //// Repeating the pattern.
					Vector3 pattern2 = new Vector3 ((x + 0.5f) * stepSize, (y + 0.5f) * stepSize, 0f);

					//color = new Color(point.x, point.y, point.z);

					//color = Color.white * Random.value; ////  noise
					//color = Color.white * method(point,frequency,true); //// pseudorandom noise  and Perlin Noise

					float sample = fractal ? Sum(method,point, frequency, octaves,lacunarity, persistence, noiseInterpolation) : method(point, frequency, noiseInterpolation);
					if (noiseType != NoiseMethodType.Pseudo) {
						sample = sample * 0.5f + 0.5f;
					}

					addGradient (gradientColor);
					color = noiseColor ? gradientColor.Evaluate (sample) : Color.white * sample;

					////each pixel psotion and color given
					texture.SetPixel (x, y, color);


				}
			}

			//// Apply all SetPixel calls
			texture.Apply();

			renderer.material.mainTexture = texture;


			break;
		case TextureType.Stripes: 
				
			pixelsPoints.Clear ();
			interpolateColorsA.Clear ();
			interpolateColorsB.Clear ();
			interpolateComplete.Clear ();
			id.Clear ();
			times.Clear ();
			renderer.material.mainTexture = null;

			texture = new Texture2D (256, 256, TextureFormat.RGB24, false);
			texture.name = "Stripes Texture";

			texture.wrapMode = TextureWrapMode.Clamp;
			texture.filterMode = FilterMode.Point;
			texture.anisoLevel = 9;


			stripeLoop = stripesFrequency ? 16 : 8;
			stripeFrequency = stripesFrequency ? 16 : 32;
			stripeResolution = (int)Mathf.Pow (stripeLoop, 2);


			int v = 0;
			for (int x = 0; x < stripeLoop; x++) {

				List<Vector2> innerArray = new List<Vector2> ();

				for (int y = 0; y < stripeLoop; y++) {


					Color c = ExtensionMethods.RandomColor ();
					int yOffset1 = (int)stripeFrequency * x;
					int yOffset2 = (resolution - (resolution - ((int)stripeFrequency * x))) + (int)stripeFrequency; 

					for (int yOff = yOffset1; yOff < yOffset2; yOff++) {

						int xOffset1 = (int)stripeFrequency * y;
						int xOffset2 = (resolution - (resolution - ((int)stripeFrequency * y))) + (int)stripeFrequency; 
						for (int xOff = xOffset1; xOff < xOffset2; xOff++) {

							//texture.SetPixel (xOff, yOff, c);
							innerArray.Add (new Vector2 (xOff, yOff));
						}

					}

					pixelsPoints.Insert (v, innerArray.ToArray ());

					interpolateColorsA.Add (Color.red);
					interpolateColorsB.Add (Color.green);
					interpolateComplete.Add (Color.black);

					id.Add (0.0f);
					times.Add (Random.Range (10f, 20f));

					v++;
					//print (v);
				}

			}
			//				///////CHANGE INDIVIDUAL BLOCKS COLOR
			//				///////INTERPOLATE BETWEEN THE COLOR 



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
		
			//texture.Apply();

			//renderer.material.mainTexture = texture;

			break;
		}


//		print ("pixals Areas: "+pixelsPoints.Count);
//		print ("color A:  "+interpolateColorsA.Count);
//		print ("color B:  "+interpolateColorsB.Count);
//		print ("color Complete:  "+interpolateComplete.Count);
//		print ("ID:  "+id.Count);
//		print ("times:  "+times.Count);
//		print ("power multiplication of stripeLoop:  " + stripeResolution);
	}


	private void UpdateStripesColor()
	{

		if (textureType == TextureType.Stripes) {


			//int i = 235;
			for (int i = 0; i < stripeResolution; i++) {

				if (id [i] < 1.0f) {
					id [i] += Time.deltaTime * (1.0f / times [i]);
				} else {
					id [i] = 0;
					times [i] = Random.Range (10f, 20f);

					interpolateColorsA [i] = interpolateComplete [i];
					interpolateColorsB [i] = colors [Random.Range (0, colors.Length - 1)];
				}

				interpolateComplete [i] = Color.Lerp (interpolateColorsA [i], interpolateColorsB [i], id [i]);
		
				//print ("i: "+r+"    "+pixelsPoints[0] [r]);
				int xMin = ((int)pixelsPoints [i] [pixelsPoints [i].Length - 1].x - (int)stripeFrequency);
				int xMax = (int)pixelsPoints [i] [pixelsPoints [i].Length - 1].x;

				int yMin = ((int)pixelsPoints [i] [pixelsPoints [i].Length - 1].y - (int)stripeFrequency);  
				int yMax = (int)pixelsPoints [i] [pixelsPoints [i].Length - 1].y;

				for (int u = yMin; u < yMax; u++) {

					for (int h = xMin; h < xMax; h++) {

						//print (texture.GetPixel (h , u ));
						texture.SetPixel (h, u, interpolateComplete [i]);
					}
				}
				
			}

			//print ("idd  "+id [20]+"   times: "+times[20]);
			//Apply all SetPixel calls
			texture.Apply ();
			renderer.material.mainTexture = texture;
		}
	}
		
}

