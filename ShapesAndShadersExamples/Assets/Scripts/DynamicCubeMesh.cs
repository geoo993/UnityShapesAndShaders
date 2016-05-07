using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]


public class DynamicCubeMesh : MonoBehaviour {

	public Camera mainCamera;
	private Ray ray;
	private RaycastHit hit;
	private GameObject hitObject = null;

	public GameObject sphere;
	private int currentSphereIndex = 0;
	private List <GameObject> newSpheres = new List<GameObject>();

	private Texture[] texture = new Texture[] {};
	private int textureIndex = 0;

	private int xlength = 0;
	private int ylength = 0;
	private int zlength = 0;

	public int xSize = 9;
	[Range(4, 40)] public int ySize = 40;
	public int zSize = 2;
	public int roundness = 0;
	public bool roundTop = false;
	public bool roundFront = false;
	public bool roundBack = false;
	public bool roundSides = false;

	private int offset = 0;
	private int midY = 0;

	private Vector3 topPoint = new Vector3 ();
	private List<int[]> controlPoints = new List<int[]>();
	private List<int> listOfIndexes = new List<int>();
	private List <Vector3> verticesCopy = new List<Vector3> ();
	private List<int> topControlPointIndexes = new List<int>();

	private BoxCollider meshCollider; 
	private MeshFilter meshFilter;
	private Renderer meshRenderer;
	private Mesh mesh;
	private Vector3[] vertices;
	private int[] triangles; 

	private Vector3[] normals;
	private Vector2[] uv;
	private static int
	SetQuad (int[] triangles, int i, int v00, int v10, int v01, int v11) {
		triangles[i] = v00;
		triangles[i + 1] = triangles[i + 4] = v01;
		triangles[i + 2] = triangles[i + 3] = v10;
		triangles[i + 5] = v11;
		return i + 6;
	}


	public enum verticesControlPrefs { sidesVertices, allVertices, none };
	public verticesControlPrefs controlType = verticesControlPrefs.allVertices;

	public enum PositioningPrefs { front, back, top, allSides };
	public PositioningPrefs verticesPrefs = PositioningPrefs.allSides;
	public bool roof = false;

	public bool addTexture = false;

	void Awake ()
	{
		textureIndex = (int)Mathf.Floor (Random.value * texture.Length);

		this.name = "dynamic object";
		CreateControllPointsIndexes ();
		MeshAndIndexes ();
		CreateSpheresInControlPoints ();


	}
	private GameObject createSphere(Vector3 pos , List <GameObject> objectArr){

		GameObject a = (GameObject) Instantiate(sphere, pos, Quaternion.identity);
		a.transform.localScale = new Vector3 (0.4f, 0.4f, 0.4f);
		a.GetComponent<Renderer> ().material.color = Color.red;
		a.transform.parent = this.transform;
		objectArr.Add (a);

		return a;
	}


	private void CreateControllPointsIndexes ()
	{
		xlength = xSize + 1;
		ylength = ySize + 1;
		zlength = zSize + 1;

		int zExtra = zSize - 2;
		offset = ((xlength * 2 ) + (zSize - 1 + zExtra)) ;

		//Debug.Log (" offset " + offset);

		for (int x = 0; x < offset + 1; x++) {

			for (int z = 0; z < zlength; z++)
			{

				List<int> innerArray = new List<int>();

				for (int y = 0; y < ylength; y++)
				{
					int myPos = (((offset * y) + x) + y);
					innerArray.Add (myPos);
					//print(innerArray[y]);
				}
				controlPoints.Insert(x, innerArray.ToArray());
			}


		}


	}
	private void CreateSpheresInControlPoints (){

		midY = (int)(Mathf.Round (ySize / 2));

		for (int s = 0; s < listOfIndexes.Count; s++) {

			//print (listOfIndexes.ToArray());
			for (int a = 0; a < offset + 1; a++) {

				if (  listOfIndexes[s].Equals(controlPoints [a] [midY])   ) {

					if (controlType == verticesControlPrefs.sidesVertices)
					{
						createSphere (verticesCopy [listOfIndexes [s]], newSpheres);
					}
					//print (listOfIndexes[s]+"   "+newSpheres.Count);
				} 

			}

		}


		if (controlType == verticesControlPrefs.sidesVertices ) {
			////create top sphere
			topPoint = new Vector3 ((float)xSize / 2, ySize, (float)zSize / 2);
			createSphere (topPoint, newSpheres);
		}

	}
	private void MeshAndIndexes () {
		
		CreateMesh ();
		CreateVertices();
		CreateTriangles();
		CreateColliders();
		AddToMesh ();
		CreateColorAndtexture ();

		int p = 0;
		for (int i = 0; i < vertices.Length; i++) {

			Vector3 vertexPos = new Vector3 (vertices [i].x + this.transform.position.x, vertices [i].y + this.transform.position.y, vertices [i].z + this.transform.position.z);
			verticesCopy.Add(vertexPos);

			listOfIndexes.Add (p);
			p++;

			if (controlType == verticesControlPrefs.allVertices)
			{
				createSphere (vertexPos, newSpheres);
			}

			if (controlType == verticesControlPrefs.sidesVertices) {
				//top vertices
				if (vertices [i].y == ySize) {
					topControlPointIndexes.Add (i);
				}
			}
		}

		//Debug.Log ("  vertices length: "+vertices.Length +"   list of indexes length: "+ listOfIndexes.Count+"  v points: "+verticesCopy.Count);


	}

	private void CreateMesh()
	{

		meshFilter = GetComponent<MeshFilter>();
		if (meshFilter == null){
			Debug.LogError("MeshFilter not found!");
			return;
		}

		mesh = meshFilter.sharedMesh;
		if (mesh == null){
			meshFilter.mesh = new Mesh();
			mesh = meshFilter.sharedMesh;
		}
		mesh.name = "building mesh";
		mesh.Clear();
	}
		
	private void CreateVertices() {


		int cornerVertices = 8;
		int edgeVertices = (xSize + ySize + zSize - 3) * 4;

		int faceVertices = (
			(xSize - 1) * (ySize - 1) +
			(xSize - 1) * (zSize - 1) +
			(ySize - 1) * (zSize - 1)) * 2;
		vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];
		normals = new Vector3[vertices.Length];
		uv = new Vector2[vertices.Length];
	
		int v = 0;
		// sides
		for (int y = 0; y <= ySize; y++) {
			for (int x = 0; x <= xSize; x++) {
				SetVertex(v++, x, y, 0);
			}
			for (int z = 1; z <= zSize; z++) {
				SetVertex(v++, xSize, y, z);
			}
			for (int x = xSize - 1; x >= 0; x--) {
				SetVertex(v++, x, y, zSize);
			}

			for (int z = zSize - 1; z > 0; z--) {
				SetVertex(v++, 0, y, z);
			}
		}


		// top 
		for (int z = 1; z < zSize; z++) {
			for (int x = 1; x < xSize; x++) {
				SetVertex(v++, x, ySize, z);
			}
		}
		//bottom
		for (int z = 1; z < zSize; z++) {
			for (int x = 1; x < xSize; x++) {
				SetVertex(v++, x, 0, z);
			}
		}



	}
	private void SetVertex (int i, int x, int y, int z) {
		Vector3 inner = vertices[i] = new Vector3(x, y, z);

		////sides
		if (x < roundness) {
			if (roundSides) {
				inner.x = roundness;
			} else {
				inner.x = 0;
			}
		}
		else if (x > xSize - roundness) {

			if (roundSides) {
				inner.x = xSize - roundness;
			} else {
				inner.x = xSize; 
			}
		}

		////top and bottom
		if (y < roundness) {
			//bottom rounder
			//inner.y = roundness;
		}
		else if (y > ySize - roundness) {
			// top rounder
			//inner.y = 0;
			if (roundTop) {
				inner.y = ySize - roundness;
			} else {
				inner.y = ySize; 
			}
		}

		////front and back
		if (z < roundness) {
			// add or disable front rounder
			if (roundFront) {
				inner.z = roundness;
			} else {
				inner.z = 0;
			}
		}
		else if (z > zSize - roundness) {
			//add or disable back rounder
			if (roundBack) {
				inner.z = zSize - roundness;
			} else {
				inner.z = zSize;
			}
		}

		normals[i] = (vertices[i] - inner).normalized;
		vertices[i] = inner + normals[i] * roundness;
		//cubeUV[i] = new Color32((byte)x, (byte)y, (byte)z, 0);
		uv[i] = new Vector2((float)x / ( xSize), (float)y / (ySize ));


	}


	private void CreateTriangles () {
		
		int quads = (xSize * ySize + xSize * zSize + ySize * zSize) * 2;
		triangles = new int[quads * 6];
		int ring = (xSize + zSize) * 2;
		int t = 0, v = 0;

		for (int y = 0; y < ySize; y++, v++) {
			for (int q = 0; q < ring - 1; q++, v++) {
				t = SetQuad(triangles, t, v, v + 1, v + ring, v + ring + 1);
			}
			t = SetQuad(triangles, t, v, v - ring + 1, v + ring, v + 1);
		}

		t = CreateTopFace(triangles, t, ring);
		t = CreateBottomFace(triangles, t, ring);

	
	}

	private int CreateTopFace (int[] triangles, int t, int ring) {
		int v = ring * ySize;
		for (int x = 0; x < xSize - 1; x++, v++) {
			t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + ring);
		}
		t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + 2);

		int vMin = ring * (ySize + 1) - 1;
		int vMid = vMin + 1;
		int vMax = v + 2;

		for (int z = 1; z < zSize - 1; z++, vMin--, vMid++, vMax++) {
			t = SetQuad(triangles, t, vMin, vMid, vMin - 1, vMid + xSize - 1);
			for (int x = 1; x < xSize - 1; x++, vMid++) {
				t = SetQuad(
					triangles, t,
					vMid, vMid + 1, vMid + xSize - 1, vMid + xSize);
			}
			t = SetQuad(triangles, t, vMid, vMax, vMid + xSize - 1, vMax + 1);
		}

		int vTop = vMin - 2;
		t = SetQuad(triangles, t, vMin, vMid, vTop + 1, vTop);
		for (int x = 1; x < xSize - 1; x++, vTop--, vMid++) {
			t = SetQuad(triangles, t, vMid, vMid + 1, vTop, vTop - 1);
		}
		t = SetQuad(triangles, t, vMid, vTop - 2, vTop, vTop - 1);

		return t;
	}

	private int CreateBottomFace (int[] triangles, int t, int ring) {
		int v = 1;
		int vMid = vertices.Length - (xSize - 1) * (zSize - 1);
		t = SetQuad(triangles, t, ring - 1, vMid, 0, 1);
		for (int x = 1; x < xSize - 1; x++, v++, vMid++) {
			t = SetQuad(triangles, t, vMid, vMid + 1, v, v + 1);
		}
		t = SetQuad(triangles, t, vMid, v + 2, v, v + 1);

		int vMin = ring - 2;
		vMid -= xSize - 2;
		int vMax = v + 2;

		for (int z = 1; z < zSize - 1; z++, vMin--, vMid++, vMax++) {
			t = SetQuad(triangles, t, vMin, vMid + xSize - 1, vMin + 1, vMid);
			for (int x = 1; x < xSize - 1; x++, vMid++) {
				t = SetQuad(
					triangles, t,
					vMid + xSize - 1, vMid + xSize, vMid, vMid + 1);
			}
			t = SetQuad(triangles, t, vMid + xSize - 1, vMax + 1, vMid, vMax);
		}

		int vTop = vMin - 1;
		t = SetQuad(triangles, t, vTop + 1, vTop, vTop + 2, vMid);
		for (int x = 1; x < xSize - 1; x++, vTop--, vMid++) {
			t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vMid + 1);
		}
		t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vTop - 2);

		return t;
	}

	private void AddToMesh()
	{
		mesh.vertices = vertices;
		mesh.triangles = triangles;

		mesh.normals = normals;
		//mesh.colors32 = cubeUV;
		mesh.uv = uv;

		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();




	}

	private void CreateColorAndtexture() {

		meshRenderer = GetComponent<MeshRenderer> ();

		if (addTexture)
		{
			//Material material = Resources.Load("Material") as Material;
			//meshRenderer.material = material;


			//Material material = new Material (Shader.Find ("Standard"));
			//Material material = new Material(Shader.Find("Self-Illumin/Diffuse"));
			Material material = new Material (Shader.Find ("Self-Illumin/Bumped Diffuse"));
			//material.color = ExtensionMethods.RandomColor();//Color.Lerp(Color.white, ExtensionMethods.RandomColor(), 1f);
	//		//material.color = Color.Lerp(Color.white, ExtensionMethods.RandomColor(), 1f);
	//
			////type 2
			texture = new Texture[] {
				Resources.Load ("TextureStripe") as Texture,
				Resources.Load ("TextureStripe1") as Texture,
				Resources.Load ("TextureStripe2") as Texture,
				Resources.Load ("TextureStripe3") as Texture,
				Resources.Load ("TextureStripe4") as Texture,
				Resources.Load ("TextureStripe5") as Texture,
				Resources.Load ("TextureStripe6") as Texture,
				Resources.Load ("TextureStripe7") as Texture,
				Resources.Load ("TextureStripe8") as Texture,
				Resources.Load ("TextureStripe9") as Texture
			};


			////type 2
	//		texture = new Texture[] {
	//
	//			Resources.Load ("windowr") as Texture,
	//			Resources.Load ("window2r") as Texture,
	//			Resources.Load ("window3r") as Texture,
	//			Resources.Load ("window4r") as Texture,
	//			Resources.Load ("window5r") as Texture,
	//			Resources.Load ("window6r") as Texture
	//		};

			//int textureIndex = (int)Mathf.Floor (Random.value * texture.Length);

			Texture2D rit = randomIllumTex (texture [textureIndex].width, texture [textureIndex].height);	
			material.SetTexture ("_MainTex", texture [textureIndex]);
			material.SetTexture ("_BumpMap", rit);

			Vector2 windowsTexture = new Vector2 (16, 32);
			Vector2 stripesTexture = new Vector2 (1, 1);

			material.SetTextureScale ("_MainTex", stripesTexture); //windowsTexture);
			material.SetTextureScale ("_BumpMap", stripesTexture);

			meshRenderer.material = material;

		}else{
			meshRenderer.material = null;
		}

	}

	Texture2D randomIllumTex(int w, int h) {

		Texture2D texture = new Texture2D(w, h , TextureFormat.Alpha8, true);
		int mipCount = texture.mipmapCount;
		texture.filterMode = FilterMode.Point;

		for( int mip = 0; mip < mipCount; ++mip ) {
			Color[] cols = texture.GetPixels( mip );

			for(int i = 0; i < cols.Length; ++i ) {
				float rand = Random.value;
				if(rand < 0.2f) rand = 0.0f;
				cols[i] = new Color(0, 0, 0, rand);
			}
			texture.SetPixels( cols, mip );
		}
		texture.Apply(true);
		return texture;
	}

	private void UpdateVerticesAndPositions() {

		////// type 1 - Control all vertices

		if (controlType == verticesControlPrefs.allVertices) {
			
			for (int i = 0; i < vertices.Length; i++) {
				vertices [i] = newSpheres [i].transform.localPosition;
			}
		}


		////// type 2 - Control either the front, back, top or all side vertices
		if (controlType == verticesControlPrefs.sidesVertices) {
			
			switch (verticesPrefs) {

			case PositioningPrefs.allSides:
			
				////all sides control point
				for (int x = 0; x < newSpheres.Count - 1; x++) {
					for (int z = 0; z < controlPoints [x].Length; z++) {
						vertices [controlPoints [x] [z]] = new Vector3 (
							newSpheres [x].transform.localPosition.x, 
							vertices [controlPoints [x] [z]].y, 
							newSpheres [x].transform.localPosition.z);

					}
				}
				break;
			case PositioningPrefs.front: 

				//// front line control point
				for (int x = 0; x < xlength; x++) {
				
					for (int z = 0; z < controlPoints [x].Length; z++) {
						vertices [controlPoints [x] [z]] = new Vector3 (
							newSpheres [x].transform.localPosition.x, 
							vertices [controlPoints [x] [z]].y, 
							newSpheres [x].transform.localPosition.z);

					}
				}

				break;
			case PositioningPrefs.back: 
			//// back line control points
				for (int x = xSize + zSize; x < newSpheres.Count - 1; x++) {

					for (int z = 0; z < controlPoints [x].Length; z++) {
						vertices [controlPoints [x] [z]] = new Vector3 (
							newSpheres [x].transform.localPosition.x, 
							vertices [controlPoints [x] [z]].y, 
							newSpheres [x].transform.localPosition.z);

					}
				}
				break;
			case PositioningPrefs.top: 

				//// top control point
				for (int y = 0; y < topControlPointIndexes.Count; y++) {

					if (roof){
						// do pointy top
						vertices [topControlPointIndexes[y]] = newSpheres [newSpheres.Count-1].transform.localPosition ;
					} else{
						//do normal top
						vertices [topControlPointIndexes[y]] = new Vector3(
							verticesCopy [topControlPointIndexes[y]].x,
							newSpheres [newSpheres.Count-1].transform.localPosition.y,
							verticesCopy [topControlPointIndexes[y]].z);
					}
				}

				break;



			}

			for (int i = 0; i < newSpheres.Count-1; i++) 
			{
				//clamp y on side spheres
				newSpheres [i].transform.localPosition = new Vector3 (
					newSpheres [i].transform.localPosition.x,
					Mathf.Clamp (newSpheres [i].transform.localPosition.y, midY, midY),
					newSpheres [i].transform.localPosition.z);
			
			}

			////clamp x and z on topsphere
			newSpheres [newSpheres.Count-1].transform.localPosition = new Vector3(
				Mathf.Clamp (newSpheres [newSpheres.Count-1].transform.localPosition.x, (float)topPoint.x, (float)topPoint.x),
			 	newSpheres [newSpheres.Count-1].transform.localPosition.y, 
				Mathf.Clamp (newSpheres [newSpheres.Count-1].transform.localPosition.z, (float)topPoint.z, (float)topPoint.z));
		
	
		}



			


	}

	private void CreateColliders () {

		Destroy(meshCollider);
		meshCollider = gameObject.AddComponent<BoxCollider>();
		//meshCollider = GetComponent<BoxCollider>();
		meshCollider.size = new Vector3(xSize, ySize, zSize);
		meshCollider.center = new Vector3 ((float)xSize/2, (float)ySize/2, (float)zSize/2);

	}
		
	void Update()
	{

		CreateMesh ();
		UpdateVerticesAndPositions ();
		CreateTriangles();
		AddToMesh();
		CreateColliders();

		CreateColorAndtexture ();

		if (Input.GetMouseButtonDown (0)) {

			ray = mainCamera.ScreenPointToRay (Input.mousePosition);

			if (Physics.Raycast (ray, out hit)) {

				CreateColorAndtexture ();

				for (int i = 0; i < newSpheres.Count; i++) {

					if (hit.collider.gameObject == newSpheres [i]) {
						hitObject = hit.collider.gameObject;

						//hitObject.GetComponent<Renderer> ().material.color = new Color (Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f));

						//currentSphereIndex = mySpheres.IndexOf(hit.collider.gameObject);
						currentSphereIndex = i;


						Debug.Log ("index: "+currentSphereIndex+"   world pos: "+ newSpheres [i].transform.position +"   local pos: "+newSpheres [i].transform.localPosition+"  current vertexpoint: "+vertices[i]);



					}
				}


			}
		}



	}


}
