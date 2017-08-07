using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_GameRoom : MonoBehaviour 
{
	public DC_Game serverGame;

	[Space(10)]
	public DynaRoom dRoom;

	[Space(10)]

	public float width;
	public float length;

	[Space(10)]
	public float gridSize = 0f;
	public int gridSpots = 0;
	public int gridWidth = 0;
	public int gridLength = 0;




	public void SetupGameRoom()
	{
		int gameMaxPlayers = serverGame.gameMaxPlayers;
		float gameGridSize = serverGame.gameGridSize;

		float dynaWidth = 0, dynaLength = 0;

		switch(gameMaxPlayers)
        {
            case 1:
            case 2:
                dynaWidth = gameGridSize;
                dynaLength = gameGridSize * 3;
            break;

            case 3:
                dynaWidth = gameGridSize * 2;
                dynaLength = gameGridSize * 3;
            break;

            case 4:
            case 5:
                dynaWidth = gameGridSize * 3;
                dynaLength = gameGridSize * 3;
            break;

            case 6:
            case 7:
            case 8:
                dynaWidth = gameGridSize * 4;
                dynaLength = gameGridSize * 4;
            break;
        }

		this.width = dynaWidth;
		this.length = dynaLength;

		this.gridSpots = gameMaxPlayers;
		this.gridSize = gameGridSize;

		this.gridWidth = (int) (width / gameGridSize);
		this.gridLength = (int) (length / gameGridSize);

		bool isS = (length == 0 || width == length);

		dRoom.width = width;
		dRoom.length = length;
		dRoom.isSquare = isS;
	}

	public void GenerateGrid()
	{
		for(int i = 0; i < gridSpots; i++)
		{
			
		}
	}
}

[System.Serializable]
public class GameGrid 
{
	public int width = 0;
	public int length = 0;

	public float size = 0;

	public int spotCount;
	public Vector3[] spots;


	GameGrid(float size, int width, int length, int spotCount)
	{
		this.size = size;
		this.width = width;
		this.length = length;	

		this.spotCount = spotCount;
	}

	public void GenerateSpots()
	{
		for(int i = 0; i < spotCount; i++)
		{
			float prog = (float) i / (float) spotCount;
		}
	}
}