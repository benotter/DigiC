using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DynaRoom : MonoBehaviour 
{
    public bool isSquare = true;
	public float width = 20.0f;
    public float length = 20.0f;

    [Space(10)]

	public float wallHeight = 3.0f;
	public float wallThickness = 1.0f;
	public bool hasRoof = false;
	public float roofHeight = 0f;

    private bool lastSquare = false;

	private float lastWidth = 0.0f;
    private float lastLength = 0.0f;
	private float lastWallHeight = 0.0f;
	private float lastWallThickness = 0.0f;
	private bool lastHasRoof = true;
	private float lastRoofHeight = 0f;

	private GameObject[] walls = new GameObject[4];
	private GameObject floor;
	private GameObject roof;

	void Update() 
	{
		if(!walls[0])
		{
			lastWidth = 0f;
			GetWalls();
		}

		if((lastHasRoof != hasRoof))
		{
			roof.SetActive(hasRoof);
			lastHasRoof = hasRoof;
            lastSquare = !isSquare;
		}
			
		if( lastSquare != isSquare ||
            width != lastWidth || 
            length != lastLength ||
			wallHeight != lastWallHeight || 
			lastWallThickness != wallThickness ||
			lastRoofHeight != roofHeight )
		{
            lastSquare = isSquare;

			lastWidth = width;
            lastLength = length;

			lastWallHeight = wallHeight;
			lastWallThickness = wallThickness;
			lastRoofHeight = roofHeight;

			UpdateWalls();
		}
	}

	void UpdateWalls()
	{
		Transform nw = walls[0].transform,
				  ew = walls[1].transform,
				  sw = walls[2].transform,
				  ww = walls[3].transform,
				  flr = floor.transform,
				  rf = roof.transform;

        flr.localScale = rf.localScale = new Vector3(
            width - wallThickness,
            wallThickness,
            (isSquare ? width : length) - wallThickness
        );

		flr.localPosition = new Vector3(0f, -(wallThickness / 2), 0f);
		rf.localPosition = new Vector3(0f, wallHeight + roofHeight + (wallThickness / 2), 0f);

        float wallH = wallHeight + (wallThickness * (hasRoof ? 2f : 1f));
        float wallW = width + wallThickness;
        float wallL = (isSquare ? width : length) - wallThickness;

        nw.localScale = sw.localScale = new Vector3(wallW, wallH, wallThickness);
		ew.localScale = ww.localScale = new Vector3(wallThickness, wallH, wallL);

        float wallY = (wallHeight / 2) - (hasRoof ? 0 : wallThickness / 2);
        float wallX = width / 2;
        float wallZ = isSquare ? wallX : length / 2;

        nw.localPosition = new Vector3(0f, wallY, wallZ);
        sw.localPosition = new Vector3(0f, wallY, -wallZ);

        ew.localPosition = new Vector3(-wallX, wallY, 0f);
        ww.localPosition = new Vector3(wallX, wallY, 0f);
    }
	void GetWalls(bool bail = false)
	{
		Transform nw_test = transform.Find("N_WALL");

		if(!nw_test && !bail)
		{
			CreateWalls();
			GetWalls(true);
			
			return;
		}

		GameObject N_WALL = transform.Find("N_WALL").gameObject,
				   E_WALL = transform.Find("E_WALL").gameObject,
				   S_WALL = transform.Find("S_WALL").gameObject,
				   W_WALL = transform.Find("W_WALL").gameObject,
				   FLOOR = transform.Find("FLOOR").gameObject,
				   ROOF = transform.Find("ROOF").gameObject;

		walls[0] = N_WALL;
		walls[1] = E_WALL;
		walls[2] = S_WALL;
		walls[3] = W_WALL;

		floor = FLOOR;
		roof = ROOF;
	}

	void CreateWalls()
	{
		GameObject N_WALL = GameObject.CreatePrimitive(PrimitiveType.Cube),
				   E_WALL = GameObject.CreatePrimitive(PrimitiveType.Cube),
				   S_WALL = GameObject.CreatePrimitive(PrimitiveType.Cube),
				   W_WALL = GameObject.CreatePrimitive(PrimitiveType.Cube),
				   FLOOR = GameObject.CreatePrimitive(PrimitiveType.Cube),
				   ROOF = GameObject.CreatePrimitive(PrimitiveType.Cube);
		
		N_WALL.name = "N_WALL";
		N_WALL.transform.parent = transform;
		N_WALL.AddComponent<Rigidbody>().isKinematic = true;

		E_WALL.name = "E_WALL";
		E_WALL.transform.parent = transform;
		E_WALL.AddComponent<Rigidbody>().isKinematic = true;

		S_WALL.name = "S_WALL";
		S_WALL.transform.parent = transform;
		S_WALL.AddComponent<Rigidbody>().isKinematic = true;

		W_WALL.name = "W_WALL";
		W_WALL.transform.parent = transform;
		W_WALL.AddComponent<Rigidbody>().isKinematic = true;
		
		FLOOR.name = "FLOOR";
		FLOOR.transform.parent = transform;
		FLOOR.AddComponent<Rigidbody>().isKinematic = true;

		ROOF.name = "ROOF";
		ROOF.transform.parent = transform;
		ROOF.AddComponent<Rigidbody>().isKinematic = true;

		if(!hasRoof)
			ROOF.SetActive(false);
	}
}
