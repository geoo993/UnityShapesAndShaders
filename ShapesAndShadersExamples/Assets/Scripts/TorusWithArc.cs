using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Rigidbody))]


public class TorusWithArc : MonoBehaviour {

	//public Color[] colourArray = new Color[256];

	[Range(1f, 10f)] public float radius = 1f; ////ringRadius
	[Range(0.05f, 1f)]public float tube = 0.3f; ////pipeRadius
	[Range(8, 72)] public int radialSegments = 24; // radius segments count or radial segments ////ringSegments
	[Range(8, 72)] public int tubularSegments = 50;//18 // also known as side segments count or ////pipeSegments

	[Range(0.0f, 6.28f)] public float arc = 4f;

	private Gradient gradientColor = new Gradient();
	MeshCollider meshCollider;
	Rigidbody ridig;

	void Awake ()
	{
		this.name = "TorusArc";
	
		meshCollider = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
		ridig = GetComponent (typeof(Rigidbody)) as Rigidbody;

		//BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
		//boxCollider.center = Vector3.zero;

	}

	private void Update ()
	{

		CreateTorus ();
	
	}
	private void CreateTorus()
	{

		List<Vector2> uvs = new List<Vector2>();
		List<Vector3> vertices = new List<Vector3>();
		List<Vector3> normals = new List<Vector3>();
		List<int> triangles = new List<int>();

		var center = new Vector3();

		MeshFilter filter = GetComponent<MeshFilter>();
		if (filter == null){
			Debug.LogError("MeshFilter not found!");
			return;
		}

		Mesh mesh = filter.sharedMesh;
		if (mesh == null){
			filter.mesh = new Mesh();
			mesh = filter.sharedMesh;
		}
		mesh.name = "TorusMesh";
		mesh.Clear ();



		for (var j = 0; j <= radialSegments; j++)
		{
			for (var i = 0; i <= tubularSegments; i++)
			{
				var u = i/(float) tubularSegments * arc;
				var v = j/(float) radialSegments * Mathf.PI * 2.0f;

				center.x = radius * Mathf.Cos(u);
				center.y = radius * Mathf.Sin(u);

				var vertex = new Vector3();
				vertex.x = (radius + tube * Mathf.Cos(v)) * Mathf.Cos(u);
				vertex.y = (radius + tube * Mathf.Cos(v)) * Mathf.Sin(u);
				vertex.z = tube * Mathf.Sin(v);

				vertices.Add(vertex);

				uvs.Add(new Vector2(i/(float) tubularSegments, j/(float) radialSegments));
				Vector3 normal = vertex - center;
				normal.Normalize();
				normals.Add(normal);
			}
		}


		for (var j = 1; j <= radialSegments; j++)
		{
			for (var i = 1; i <= tubularSegments; i++)
			{
				var a = (tubularSegments + 1) * j + i - 1;
				var b = (tubularSegments + 1) * (j - 1) + i - 1;
				var c = (tubularSegments + 1) * (j - 1) + i;
				var d = (tubularSegments + 1) * j + i;

				triangles.Add(a);
				triangles.Add(b);
				triangles.Add(d);

				triangles.Add(b);
				triangles.Add(c);
				triangles.Add(d);
			}

		}

		mesh.vertices = vertices.ToArray();
		mesh.normals = normals.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.triangles = triangles.ToArray();

		mesh.RecalculateNormals();
		CalculateTangent.TangentSolver (mesh);

		mesh.RecalculateBounds();
		mesh.Optimize();

		//MeshRenderer renderer = GetComponent<MeshRenderer> ();
		//renderer.material.color = Color.red;

		//addColorTexture (renderer);

		//print (vertices.Count);

		ridig.isKinematic = true;
		meshCollider.sharedMesh = mesh;

	}


	private void addColorTexture( MeshRenderer renderer)
	{
		Texture2D colourPalette = new Texture2D(256, 256, TextureFormat.ARGB32, false);

		for(int x = 0; x < 256; x++){
			for(int y = 0; y < 256; y++){
				addGradient (gradientColor);
				colourPalette.SetPixel(x,y, gradientColor.Evaluate(0.5f));
			}
		}
		colourPalette.filterMode = FilterMode.Point;
		colourPalette.wrapMode = TextureWrapMode.Clamp;
		colourPalette.Apply();
		renderer.material.SetTexture("_ColorRamp",colourPalette);
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
}
