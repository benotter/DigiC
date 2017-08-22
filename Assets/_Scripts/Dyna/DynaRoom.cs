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

	public bool hasFloor = true;

    [Space(10)]

	public bool hasWalls = true;

	public float wallHeight = 3.0f;
	public float wallThickness = 1.0f;

	[Space(10)]

	public bool hasRoof = false;
	public float roofHeight = 0f;

	[Space(10)]

	public bool hasWindows = false;

    private bool lastSquare = false;

	private float lastWidth = 0.0f;
    private float lastLength = 0.0f;
	private float lastWallHeight = 0.0f;
	private float lastWallThickness = 0.0f;
	private float lastRoofHeight = 0f;

	private bool lastHasWalls = true;
	private bool lastHasFloor = true;
	private bool lastHasRoof = true;
	private bool lastHasWindows = false;

	

	private GameObject floor;
	private GameObject roof;

	private GameObject[] walls = new GameObject[4];
	private GameObject[] windows = new GameObject[4];


	void Update() 
	{
		if(!walls[0] || !windows[0] || !floor || !roof)
		{
			lastWidth = 0f;
			GetWalls();
		}

		if( lastHasFloor != hasFloor ) 
		{
			floor.SetActive(hasFloor);
			lastHasFloor = hasFloor;

			// Forces Recalc
			lastSquare = !isSquare;
		}

		if( lastHasRoof != hasRoof )
		{
			roof.SetActive(hasRoof);
			lastHasRoof = hasRoof;
			
            lastSquare = !isSquare;
		}

		if( lastHasWalls != hasWalls )
		{
			foreach(GameObject wall in walls)
				wall.SetActive(hasWalls);
				
			lastHasWalls = hasWalls;

			lastSquare = !isSquare;
		}

		if( lastHasWindows != hasWindows ) 
		{
			foreach(GameObject win in windows)
				win.SetActive(hasWindows);

			lastHasWindows = hasWindows;

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

			UpdateFloorRoof();
			UpdateWalls();
			UpdateWindows();
		}
	}

	void UpdateWalls()
	{
		Transform nw = walls[0].transform,
				  ew = walls[1].transform,
				  sw = walls[2].transform,
				  ww = walls[3].transform;

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

	void UpdateWindows()
	{
		Transform nw = windows[0].transform,
				  ew = windows[1].transform,
				  sw = windows[2].transform,
				  ww = windows[3].transform;

        float wallH = roofHeight;
        float wallW = width + wallThickness;
        float wallL = (isSquare ? width : length) - wallThickness;

        nw.localScale = sw.localScale = new Vector3(wallW, wallH, wallThickness);
		ew.localScale = ww.localScale = new Vector3(wallThickness, wallH, wallL);

        float wallY = (wallHeight + ( wallH / 2 ) ) + (hasRoof ? wallThickness : 0f);
        float wallX = width / 2;
        float wallZ = isSquare ? wallX : length / 2;

        nw.localPosition = new Vector3(0f, wallY, wallZ);
        sw.localPosition = new Vector3(0f, wallY, -wallZ);

        ew.localPosition = new Vector3(-wallX, wallY, 0f);
        ww.localPosition = new Vector3(wallX, wallY, 0f);
	}

	void UpdateFloorRoof()
	{
		Transform flr = floor.transform,
				   rf = roof.transform;

        flr.localScale = rf.localScale = new Vector3(
            width - wallThickness,
            wallThickness,
            (isSquare ? width : length) - wallThickness
        );

		flr.localPosition = new Vector3(0f, -(wallThickness / 2), 0f);
		rf.localPosition = new Vector3(0f, wallHeight + roofHeight + (wallThickness / 2), 0f);
	}

	void GetWalls(bool bail = false)
	{
		Transform nw_test = transform.Find("N_WALL");
		Transform nwin_test = transform.Find("N_WIN");

		if(!nw_test || !nwin_test)
			CreateWalls();
		
		GameObject N_WALL = transform.Find("N_WALL").gameObject,
				   E_WALL = transform.Find("E_WALL").gameObject,
				   S_WALL = transform.Find("S_WALL").gameObject,
				   W_WALL = transform.Find("W_WALL").gameObject;

		GameObject N_WIN = transform.Find("N_WIN").gameObject,
				   E_WIN = transform.Find("E_WIN").gameObject,
				   S_WIN = transform.Find("S_WIN").gameObject,
				   W_WIN = transform.Find("W_WIN").gameObject;

		GameObject FLOOR = transform.Find("FLOOR").gameObject,
				   ROOF = transform.Find("ROOF").gameObject;

		walls[0] = N_WALL;
		walls[1] = E_WALL;
		walls[2] = S_WALL;
		walls[3] = W_WALL;

		windows[0] = N_WIN;
		windows[1] = E_WIN;
		windows[2] = S_WIN;
		windows[3] = W_WIN;

		floor = FLOOR;
		roof = ROOF;
	}

	void CreateWalls()
	{
		GameObject N_WALL = GameObject.CreatePrimitive(PrimitiveType.Cube),
				   E_WALL = GameObject.CreatePrimitive(PrimitiveType.Cube),
				   S_WALL = GameObject.CreatePrimitive(PrimitiveType.Cube),
				   W_WALL = GameObject.CreatePrimitive(PrimitiveType.Cube);

		GameObject N_WIN = GameObject.CreatePrimitive(PrimitiveType.Cube),
				   E_WIN = GameObject.CreatePrimitive(PrimitiveType.Cube),
				   S_WIN = GameObject.CreatePrimitive(PrimitiveType.Cube),
				   W_WIN = GameObject.CreatePrimitive(PrimitiveType.Cube);

		GameObject FLOOR = GameObject.CreatePrimitive(PrimitiveType.Cube),
				   ROOF = GameObject.CreatePrimitive(PrimitiveType.Cube);

		SetupWall(N_WALL, "N_WALL");
		SetupWall(E_WALL, "E_WALL");
		SetupWall(S_WALL, "S_WALL");
		SetupWall(W_WALL, "W_WALL");

		SetupWall(N_WIN, "N_WIN");
		SetupWall(E_WIN, "E_WIN");
		SetupWall(S_WIN, "S_WIN");
		SetupWall(W_WIN, "W_WIN");

		SetupWall(FLOOR, "FLOOR");
		SetupWall(ROOF, "ROOF");

		if(!hasFloor)
			FLOOR.SetActive(false);

		if(!hasRoof)
			ROOF.SetActive(false);

		if(!hasWalls)
		{
			N_WALL.SetActive(false);
			E_WALL.SetActive(false);
			S_WALL.SetActive(false);
			W_WALL.SetActive(false);
		}

		if(!hasWindows)
		{
			N_WIN.SetActive(false);
			E_WIN.SetActive(false);
			S_WIN.SetActive(false);
			W_WIN.SetActive(false);
		}
	}

	void SetupWall(GameObject wall, string name)
	{
		wall.name = name;
		wall.transform.parent = transform;
		wall.AddComponent<Rigidbody>().isKinematic = true;
	}
}
