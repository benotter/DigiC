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

[System.Serializable]
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
	public float gridCellSize = 20;

	[HideInInspector]
	public SyncListGridCell cells = new SyncListGridCell();
}

public partial class DC_GameGrid
{
	public int GetPosInt(int x, int y)
	{
		return (((x - 1) * 3) + y) - 1;
	}
	public bool CheckPosition(int x, int y, GameObject go = null)
	{
		if(x < 1 || x > 3 || y < 1 || y > 3)
			return false;

		if(!go)
			return (cells[GetPosInt(x, y)].player != null);
		else
			return (cells[GetPosInt(x, y)].player == go);
	}

	public bool SetPosition(GameObject playerO, int x, int y)
	{
		if(CheckPosition(x, y, playerO))
			return false;
		
		int pos = GetPosInt(x, y);

		Debug.Log("Setting X: " + x + ", Y: " + y);
		Debug.Log("Array Position: " + pos);

		if(pos >= cells.Count || pos < 0)
			return false;

		DC_Player player = playerO.GetComponent<DC_Player>();

		if(CheckPosition(player.gameGridX, player.gameGridY, playerO))
		{
			int oldPos = GetPosInt(player.gameGridX, player.gameGridY);

			if(oldPos >= 0 && oldPos < cells.Count)
			{
				var oldCell = cells[oldPos];

				oldCell.player = null;
				cells[oldPos] = oldCell;
			}			
		}

		player.gameGridX = x;
		player.gameGridY = y;

		GameGridCell cell = cells[pos];
		cell.player = playerO;
		cells[pos] = cell;

		playerO.transform.position = cell.cellPos;

		return true;
	}

	public void ClearPosition(int x, int y, GameObject go)
	{
		int pos = GetPosInt(x, y);

		if(pos >= cells.Count || pos < 0)
			return;

		GameGridCell cell = cells[pos];

		if(go)
		{
			if(cell.player == go)
				cell.player = null;	
		}
		else
			cell.player = null;

		cells[pos] = cell;
	}

	public void UpdateGameGrid()
    {
        dynaRoom.width = gridCellSize * 3;
        dynaRoom.length = gridCellSize * 3;
    }
}
