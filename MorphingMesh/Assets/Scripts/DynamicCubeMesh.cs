using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(BoxCollider))]


public class DynamicCubeMesh : MonoBehaviour {

	public Camera mainCamera;
	private Ray ray;
	private RaycastHit hit;
	private GameObject hitObject = null;

	List <GameObject> mySpheres = new List<GameObject>();

	public GameObject sphere;
	private int sphereCurrentIndex = 0;

	public int xSize = 3;
	public int ySize = 2;
	public int zSize = 4;

	private BoxCollider meshCollider; 
	private MeshFilter meshFilter;
	private Mesh mesh;
	private Vector3[] vertices;
	private int[] triangles; 

	private int[] trianglesX;
	private int[] trianglesY;
	private int[] trianglesZ;

	private Vector3[] normals;
	private Color32[] cubeUV;
	private static int
	SetQuad (int[] triangles, int i, int v00, int v10, int v01, int v11) {
		triangles[i] = v00;
		triangles[i + 1] = triangles[i + 4] = v01;
		triangles[i + 2] = triangles[i + 3] = v10;
		triangles[i + 5] = v11;
		return i + 6;
	}

	void Awake ()
	{
		this.name = "dynamic object";

		vertexPoints ();
	}


	//	private void OnDrawGizmos () {
	//		
	//		if (vertices == null) {
	//			return;
	//		}
	//		Gizmos.color = Color.yellow;
	//
	//		for (int i = 0; i < vertices.Length; i++) {
	//			Gizmos.DrawSphere(vertices[i] + this.transform.position, 0.1f);
	//		}
	//
	//	}

	private void vertexPoints () {
		
		CreateMesh ();
		CreateVertices();
		CreateTriangles();
		CreateColliders();
		AddToMesh ();

		for (int i = 0; i < vertices.Length; i++) {

			Vector3 verticesPositions = new Vector3(vertices [i].x, vertices [i].y + this.transform.position.y, vertices [i].z);
			GameObject a = (GameObject) Instantiate(sphere, verticesPositions, Quaternion.identity);
			a.transform.localScale = new Vector3 (0.4f, 0.4f, 0.4f);
			a.GetComponent<Renderer> ().material.MaterialColorToRandom();
			a.transform.parent = this.transform;
			mySpheres.Add (a);

		}
		Debug.Log ("  vertices length: "+vertices.Length +"   sphere objects length: "+ mySpheres.Count);

		for (int a = 0; a < mySpheres.Count; a++) {

			Debug.Log ("index: " + a + "  position: " + mySpheres[a].transform.position);
		}

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
		mesh.name = "dynamic mesh";
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
		cubeUV = new Color32[vertices.Length];

		int v = 0;

		// sides
		for (int y = 0; y <= ySize; y++) {
			for (int x = 0; x <= xSize; x++) {
				vertices[v++] = new Vector3(x, y, 0);
				//yield return wait;
			}
			for (int z = 1; z <= zSize; z++) {
				vertices[v++] = new Vector3(xSize, y, z);
				//yield return wait;
			}
			for (int x = xSize - 1; x >= 0; x--) {
				vertices[v++] = new Vector3(x, y, zSize);
				//yield return wait;
			}
			for (int z = zSize - 1; z > 0; z--) {
				vertices[v++] = new Vector3(0, y, z);
				//yield return wait;
			}
		}

		// top and bottom
		for (int z = 1; z < zSize; z++) {
			for (int x = 1; x < xSize; x++) {
				vertices[v++] = new Vector3(x, ySize, z);
				//yield return wait;
			}
		}
		for (int z = 1; z < zSize; z++) {
			for (int x = 1; x < xSize; x++) {
				vertices[v++] = new Vector3(x, 0, z);
				//yield return wait;
			}
		}


	}

//	private void CreateTriangles () {
//		
//		trianglesX = new int[(ySize * zSize) * 12];
//		trianglesY = new int[(xSize * zSize) * 12];
//		trianglesZ = new int[(xSize * ySize) * 12];
//
//		int ring = (xSize + zSize) * 2;
//		int tZ = 0, tX = 0, tY = 0, v = 0;
//
//		for (int y = 0; y < ySize; y++, v++) {
//			for (int q = 0; q < xSize; q++, v++) {
//				tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
//			}
//			for (int q = 0; q < zSize; q++, v++) {
//				tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
//			}
//			for (int q = 0; q < xSize; q++, v++) {
//				tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
//			}
//			for (int q = 0; q < zSize - 1; q++, v++) {
//				tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
//			}
//			tX = SetQuad(trianglesX, tX, v, v - ring + 1, v + ring, v + 1);
//		}
//
//		tY = CreateTopFace(trianglesY, tY, ring);
//		tY = CreateBottomFace(trianglesY, tY, ring);
//
//	}

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

//		mesh.subMeshCount = 3;
//		mesh.SetTriangles(trianglesZ, 0);
//		mesh.SetTriangles(trianglesX, 1);
//		mesh.SetTriangles(trianglesY, 2);

		mesh.normals = normals;
		mesh.colors32 = cubeUV;

		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();


		GetComponent<MeshRenderer> ().material.color = Color.blue;
	}

	private void UpdateVertices() {

		for (int i = 0; i < vertices.Length; i++) {

			vertices [i] = mySpheres [i].transform.localPosition; //+ this.transform.position;
		}

	}

	private void CreateColliders () {

		meshCollider = GetComponent<BoxCollider>();
		meshCollider.size = new Vector3(xSize, ySize, zSize);
		meshCollider.center = new Vector3 (xSize/2, ySize/2, zSize/2);

	}
		
	void Update()
	{

		CreateMesh ();
		UpdateVertices ();
		CreateTriangles();
		AddToMesh();
		CreateColliders();

		if (Input.GetMouseButtonDown (0)) {

			ray = mainCamera.ScreenPointToRay (Input.mousePosition);

			if (Physics.Raycast (ray, out hit)) {


				for (int i = 0; i < mySpheres.Count; i++) {

					if (hit.collider.gameObject == mySpheres [i]) {
						hitObject = hit.collider.gameObject;

						//hitObject.GetComponent<Renderer> ().material.color = new Color (Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f));

						sphereCurrentIndex = mySpheres.IndexOf(hit.collider.gameObject);
						//vertices [i] = mySphere [i].transform.position;
						//Debug.Log ("index: "+ currentSphereIndex +"   position: "+hitObject.transform.position);
						Debug.Log ("index: "+sphereCurrentIndex+"   world pos: "+ mySpheres [i].transform.position +"   local pos: "+mySpheres [i].transform.localPosition+"  current vertexpoint: "+vertices[i]);

					}
				}

			
			}
		}



	}


}
