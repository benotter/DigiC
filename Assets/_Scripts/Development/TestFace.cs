using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class TestFace : MonoBehaviour 
{
	public Texture image;

	public int width = 50;
	public int height = 50;

	[Space(10)]

	public bool genMesh = false;

	private MeshFilter meshF;
	private MeshRenderer meshR;

	private Texture lastImage;

	private Mesh mesh;

	void Start () 
	{
		
	}
	
	void Update () 
	{
		if(!mesh)
		{
			mesh = new Mesh();
			mesh.name = "Gen'd Mesh";
			mesh.MarkDynamic();
		}

		if(!meshF)
		{
			meshF = GetComponent<MeshFilter>();
			meshF.mesh = mesh;	
		}

		if(!meshR)
			meshR = GetComponent<MeshRenderer>();

		if(genMesh)
		{
			GenMesh();
			genMesh = false;
		}
	}

	void GenMesh()
	{
		int area = ( width * height ) * 4;

		Vector3[] verts = new Vector3[area];
		Vector2[] uvs = new Vector2[area];
		int[] tris = new int[area * 3];

		int vertC = 0;
		int trisC = 0;

		for(int x = 0; x < width; x++)
		{
			for(int y = 0; y < height; y++)
			{
				AddFace(x, y, ref verts, ref uvs, ref tris, ref vertC, ref trisC);
			}
		}

		Debug.Log(vertC);

		mesh.SetVertices(new List<Vector3>(verts));
		mesh.uv = uvs;
		mesh.triangles = tris;
		
		mesh.RecalculateNormals();
		mesh.RecalculateTangents();
		mesh.RecalculateBounds();

		mesh.UploadMeshData(false);

		Debug.Log("Verts: " + mesh.vertexCount);
	}

	void AddFace(int oX, int oY, ref Vector3[] verts, ref Vector2[] uvs, ref int[] tris, ref int vertC, ref int trisC)
	{
		float newH = (height / (float) (height / (float) width));

		float xUnit = (float) ( 1f / (float) width );
		float yUnit = (float) ( 1f / newH );

		float xP = ( oX / (float) (width)) - 0.5f;
		float yP = ( oY  / newH) - ( (height / (float) width) / 2f );

		Vector3 v1 = new Vector3(xP, 0f, yP),
				v2 = new Vector3(xP + xUnit, 0f, yP),
				v3 = new Vector3(xP + xUnit, 0f, yP + yUnit),
				v4 = new Vector3(xP, 0f, yP + yUnit);

		int t1 = 0 + vertC,
			t2 = 1 + vertC,
			t3 = 2 + vertC,
			t4 = 3 + vertC;

		vertC += 4;

		verts[t1] = v1;
		verts[t2] = v2;
		verts[t3] = v3;
		verts[t4] = v4;

		float sX = (oX) / (float) (width);
		float sY = (oY) / (float) (height);

		float xT = xUnit * ( 0.5f);
		float yT = (1f / (float) height) * (0.5f);

		uvs[t1] = new Vector2( sX + xT, sY + yT );
		uvs[t2] = new Vector2(sX + xT, sY + yT);
		uvs[t3] = new Vector2(sX + xT, sY + yT);
		uvs[t4] = new Vector2(sX + xT, sY + yT);

		tris[trisC + 0] = t1;
		tris[trisC + 1] = t3;
		tris[trisC + 2] = t2;

		tris[trisC + 3] = t4;
		tris[trisC + 4] = t3;
		tris[trisC + 5] = t1;

		trisC += 6;
	}
}
