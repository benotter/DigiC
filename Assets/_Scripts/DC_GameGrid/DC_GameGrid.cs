using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public struct GameGridCell 
{
	public GameObject player;
	public Vector3 cellPos;
	public int x;
	public int y;
	public GameGridCell SetPos(int x, int y)
	{
		this.x = x;
		this.y = y;

		return this;
	}
}
public class SyncListGridCell : SyncListStruct<GameGridCell>{}

public partial class DC_GameGrid : NetworkBehaviour
{
	[System.Serializable]
	public enum GridSize 
	{
		One,
		Two,
		Three
	}

	[SyncVar]
	public GridSize gridSize = GridSize.One;

	[SyncVar]
	public float gridCellSize;

	[HideInInspector]
	public SyncListGridCell cells;
}

public partial class DC_GameGrid
{
	public bool CheckPosition(int x, int y)
	{
		if(cells.Count < 1)
			return false;

		if(x < 1 || x > 3 || y < 1 || y > 3)
			return false;

		return (cells[x + (3 * y)].player != null);
	}

	public bool SetPosition(GameObject playerO, int x, int y)
	{
		if(CheckPosition(x, y))
			return false;
		
		int pos = x + (3 * y);

		GameGridCell cell = cells[pos];
		cell.player = playerO;
		cells[pos] = cell;

		playerO.transform.position = cell.cellPos;

		return true;
	}

	public void UpdateGameGrid()
    {
        dynaRoom.width = gridCellSize * 3;
        dynaRoom.length = gridCellSize * 3;
    }
}
