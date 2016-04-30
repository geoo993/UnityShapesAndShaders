using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
//[RequireComponent(typeof(MeshCollider))]

public class MeshObject : MonoBehaviour {


	[Range(-5.0f,5.0f)] public float bottomlengthX = 1f; 
	[Range(-5.0f,5.0f)] public float bottomlengthY = 1f; 
	[Range(-5.0f,5.0f)] public float bottomlengthZ = 1f;  
	[Range(-5.0f,5.0f)] public float midlengthX = 1f; 
	[Range(-5.0f,5.0f)] public float midlengthY = 0f; 
	[Range(-5.0f,5.0f)] public float midlengthZ = 1f; 
	[Range(-5.0f,5.0f)] public float toplengthX = 1f; 
	[Range(-5.0f,5.0f)] public float toplengthY = 1f; 
	[Range(-5.0f,5.0f)] public float toplengthZ = 1f; 

	Vector3 v0 = Vector3.zero; //left (bottom, front, left)
	Vector3 v1 = Vector3.zero; //right (bottom, front, right)
	Vector3 v2 = Vector3.zero; //right (bottom, back, right)
	Vector3 v3 = Vector3.zero; //left (bottom, back, left)

	Vector3 v4 = Vector3.zero; //left (top, front, left)
	Vector3 v5 = Vector3.zero; //right (top, front, right)
	Vector3 v6 = Vector3.zero; //right (top, back, right)
	Vector3 v7 = Vector3.zero; //left (top, back, left)

	Vector3 v8 = Vector3.zero; //left (mid, front, left)
	Vector3 v9 = Vector3.zero; //right (mid, front, right)
	Vector3 v10 = Vector3.zero; //right (mid, back, right)
	Vector3 v11 = Vector3.zero; //left (mid, back, left)

	Vector3 v12 = Vector3.zero; //right +2 (top, front, right)
	Vector3 v13 = Vector3.zero; //right +2 (top, back, right)
	Vector3 v14 = Vector3.zero; //right +2 (mid, front, right)
	Vector3 v15 = Vector3.zero; //right +2 (mid, back, right)
	Vector3 v16 = Vector3.zero; //right +2 (bottom, front, right)
	Vector3 v17 = Vector3.zero; //right +2 (bottom, back, right)


	public Camera mainCamera;
	private Ray ray;
	private RaycastHit hit;
	private GameObject hitObject = null;

	List <GameObject> mySphere = new List<GameObject>();

	public GameObject sphere;

	private int currentSphereIndex = 0;

	private Vector3[] vertices;

	void Awake ()
	{

		this.name = "mesh object";
		createVerticesPoints();
		vertexPoints ();

		//StartCoroutine (vertexPoints ());

		CreateCube ();	
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
	//private IEnumerator Generate () {

		//yield return wait;


		for (int i = 0; i < vertices.Length; i++) {

			Vector3 verticesPositions = new Vector3(vertices [i].x, vertices [i].y + this.transform.position.y, vertices [i].z);
			GameObject a = (GameObject) Instantiate(sphere, verticesPositions, Quaternion.identity);
			a.transform.localScale = new Vector3 (0.4f, 0.4f, 0.4f);
			a.GetComponent<Renderer> ().material.MaterialColorToRandom();
			a.transform.parent = this.transform;
			mySphere.Add (a);


			//yield return wait;
		}
		Debug.Log ("  vert: "+vertices.Length +"   sphere: "+ mySphere.Count);

		//yield return wait;

		for (int a = 0; a < mySphere.Count; a++) {

			Debug.Log ("index: " + a + "   position: " + mySphere[a].transform.position);
		}

	}


	void Update()
	{

		//cube details
		v0 = mySphere[0].transform.position; //left (top, front, left)
		v1 = mySphere[1].transform.position;  //right (top, front, right)
		v2 = mySphere[2].transform.position;  //right (top, back, right)
		v3 = mySphere[3].transform.position;  //left (top, back, left)

		v4 = mySphere[4].transform.position;  //left (mid, front, left)
		v5 = mySphere[5].transform.position;  //right (mid, front, right)
		v6 = mySphere[6].transform.position;  //right (mid, back, right)
		v7 = mySphere[7].transform.position;  //left (mid, back, left)

		v8 = mySphere[8].transform.position; ; //left (bottom, front, left)
		v9 = mySphere[9].transform.position;  //right (bottom, front, right)
		v10 = mySphere[10].transform.position;  //right (bottom, back, right)
		v11 = mySphere[11].transform.position;  //left (bottom, back, left)

		v12 = mySphere[11].transform.position; //right +2 (top, front, right)
		v13 = mySphere[11].transform.position; //right +2 (top, back, right)
		v14 = mySphere[11].transform.position; //right +2 (mid, front, right)
		v15 = mySphere[11].transform.position; //right +2 (mid, back, right)
		v16 = mySphere[11].transform.position; //right +2 (bottom, front, right)
		v17 = mySphere[11].transform.position; //right +2 (bottom, back, right)


		CreateCube ();


		if (Input.GetMouseButtonDown (0)) {

			ray = mainCamera.ScreenPointToRay (Input.mousePosition);

			if (Physics.Raycast (ray, out hit)) {

				Debug.Log (vertices [0]);

				vertices [0] = new Vector3 (7, 9, 4);

				Debug.Log (vertices [0]);
				for (int i = 0; i < mySphere.Count; i++) {

					if (hit.collider.gameObject == mySphere [i]) {
						hitObject = hit.collider.gameObject;

						hitObject.GetComponent<Renderer> ().material.color = new Color (Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f));

						//currentSphereIndex = mySphere.IndexOf(hit.collider.gameObject);
						//vertices [i] = mySphere [i].transform.position;
						//Debug.Log ("index: "+ currentSphereIndex +"   position: "+hitObject.transform.position);

					}
				}

//				if (hit.collider.gameObject.name == "building") {
//					
//				
//				}
				//hitObject = hit.collider.gameObject;
				//Debug.Log (hit.collider.name);

			}
		}



	}

	private void createVerticesPoints(){

		//cube vertices details
		v0 = new Vector3 (-toplengthX, toplengthY, toplengthZ); //left (top, front, left)
		v1 = new Vector3 (toplengthX, toplengthY, toplengthZ); //right (top, front, right)
		v2 = new Vector3 (toplengthX , toplengthY, -toplengthZ); //right (top, back, right)
		v3 = new Vector3 (-toplengthX , toplengthY, -toplengthZ); //left (top, back, left)

		v4 = new Vector3 (-midlengthX, midlengthY, midlengthZ ); //left (mid, front, left)
		v5 = new Vector3 (midlengthX, midlengthY, midlengthZ ); //right (mid, front, right)
		v6 = new Vector3 (midlengthX , midlengthY, -midlengthZ); //right (mid, back, right)
		v7 = new Vector3 (-midlengthX , midlengthY , -midlengthZ); //left (mid, back, left)

		v8 = new Vector3 (-bottomlengthX, -bottomlengthY, bottomlengthZ ); //left (bottom, front, left)
		v9 = new Vector3 (bottomlengthX, -bottomlengthY, bottomlengthZ ); //right (bottom, front, right)
		v10 = new Vector3 (bottomlengthX , -bottomlengthY, -bottomlengthZ); //right (bottom, back, right)
		v11 = new Vector3 (-bottomlengthX , -bottomlengthY , -bottomlengthZ); //left (bottom, back, left)

		//		v12 = new Vector3 (toplengthX * 2.5f, toplengthY, toplengthZ); //right +2 (top, front, right)
		//		v13 = new Vector3 (toplengthX * 2.5f, toplengthY, -toplengthZ); //right +2 (top, back, right)
		//		v14 = new Vector3 (midlengthX * 2.5f, midlengthY, midlengthZ ); //right +2 (mid, front, right)
		//		v15 = new Vector3 (midlengthX * 2.5f, midlengthY, -midlengthZ); //right +2 (mid, back, right)
		//		v16 = new Vector3 (bottomlengthX * 2.5f, -bottomlengthY, bottomlengthZ ); //right +2 (bottom, front, right)
		//		v17 = new Vector3 (bottomlengthX * 2.5f, -bottomlengthY, -bottomlengthZ); //right +2 (bottom, back, right)
		//
		vertices = new Vector3[] {v0,v1,v2,v3,v4,v5,v6,v7,v8,v9,v10,v11};

		//vertices = new Vector3[] {v0,v1,v2,v3,v4,v5,v6,v7,v8,v9,v10,v11,v12,v13,v14,v15,v16,v17};

	}

	private void CreateCube()
	{

		MeshFilter meshFilter = GetComponent<MeshFilter>();
		if (meshFilter==null){
			Debug.LogError("MeshFilter not found!");
			return;
		}

		Mesh mesh = meshFilter.sharedMesh;
		//mesh = meshFilter.mesh;
		if (mesh == null){
			meshFilter.mesh = new Mesh();
			mesh = meshFilter.sharedMesh;
			//mesh = meshFilter.mesh;
		}

		mesh.Clear();


		//Add region Vertices
		mesh.vertices = new Vector3[]{


			// top Front face 
			v0, v1, v4, v5,

			// top Back face 
			v2, v3, v6, v7,

			// top Left face 
			v3, v0, v7, v4,

			// top Right face
			v1, v2, v5, v6,



			// bottom Front face 
			v4, v5, v8, v9,


			// bottom Back face 
			v6, v7, v10, v11,

			// bottom Left face 
			v7, v4, v11, v8,

			// bottom Right face
			v5, v6, v9, v10,




			// Top face 
			v3, v2, v0, v1,

			// Bottom face 
			v8, v9, v11, v10




		};
		//end vertices region

		//Add Triangles region 
		//these are three point, and work clockwise to determine what side is visible
		mesh.triangles = new int[]{

			// top front face
			0,2,3, // first triangle
			3,1,0, // second triangle

			// top back face
			4,6,7, // first triangle
			7,5,4, // second triangle

			// top left face
			8,10,11, // first triangle
			11,9,8, // second triangle

			// top right face
			12,14,15, // first triangle
			15,13,12, // second triangle


			//bottom front face
			16,18,19, // first triangle
			19,17,16, // second triangle

			//bottom back face
			20,22,23, // first triangle
			23,21,20, // second triangle

			//bottom left face 
			24,26,27, // first triangle
			27,25,24, // second triangle

			//bottom right face 
			28,30,31, // first triangle
			31,29,28, // second triangle



			//top face 
			32,34,35, // first triangle
			35,33,32, // second triangle

			//bottom face 
			36,38,39, // first triangle
			39,37,36 // second triangle

		};
		//end triangles region


		//Normales vector3 region
		Vector3 front 	= Vector3.forward;
		Vector3 back 	= Vector3.back;
		Vector3 left 	= Vector3.left;
		Vector3 right 	= Vector3.right;
		Vector3 up 		= Vector3.up;
		Vector3 down 	= Vector3.down;

		//Add Normales region
		mesh.normals = new Vector3[]
		{
			// top Front face
			front, front, front, front,

			// top Back face
			back, back, back, back,

			// top Left face
			left, left, left, left,

			// top Right face
			right, right, right, right,


			// bottom Front face
			front, front, front, front,

			// bottom Back face
			back, back, back, back,

			// bottom Left face
			left, left, left, left,

			// bottom Right face
			right, right, right, right,



			// Top face
			up, up, up, up,

			// Bottom face
			down, down, down, down

		};
		//end Normales region

		//Add vector2 regions 
		Vector2 u00 = new Vector2( 0f, 0f );
		Vector2 u10 = new Vector2( 1f, 0f );
		Vector2 u01 = new Vector2( 0f, 1f );
		Vector2 u11 = new Vector2( 1f, 1f );

		//Add UVs region 
		mesh.uv = new Vector2[]
		{
			// Front face uv
			u01, u00, u11, u10,

			// Back face uv
			u01, u00, u11, u10,

			// Left face uv
			u01, u00, u11, u10,

			// Right face uv
			u01, u00, u11, u10,



			// Front face uv
			u01, u00, u11, u10,

			// Back face uv
			u01, u00, u11, u10,

			// Left face uv
			u01, u00, u11, u10,

			// Right face uv
			u01, u00, u11, u10,


			// Top face uv
			u01, u00, u11, u10,

			// Bottom face uv
			u01, u00, u11, u10
		};
		//End UVs region
		

//		Texture texture = Resources.Load ("TextureComplete1") as Texture;
//		material.mainTexture = texture;


		//MeshCollider meshCollider = cube.AddComponent<MeshCollider> ();
		//meshCollider.isTrigger = false;


		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();


		GetComponent<MeshRenderer> ().material.color = Color.blue;
		//GetComponent<MeshRenderer> ().material.MaterialColorToRandom ();
		//MeshRenderer meshRenderer = GetComponent<MeshRenderer> ();
//		Material material = new Material (Shader.Find ("Standard"));
//		material.MaterialColorToRandom ();
//		meshRenderer.material = material;
	}


}
