using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_GameGrid : NetworkBehaviour
{
	public DynaRoom dynaRoom;
	public DC_GridSelector gridSelector;


	[SyncVar]
	public GridSize gridSize = GridSize.One;

	[SyncVar]
	public float gridCellSize = 20;

	[HideInInspector]
	public SyncListGridCell cells = new SyncListGridCell();

	public override void OnStartClient()
    {
        UpdateGameGrid();
    }

	public override void OnStartServer()
    {
        cells.Add(new GameGridCell().SetPos(1, 1)); // 0
        cells.Add(new GameGridCell().SetPos(1, 2)); // 1
        cells.Add(new GameGridCell().SetPos(1, 3)); // 2

        cells.Add(new GameGridCell().SetPos(2, 1)); // 3
        cells.Add(new GameGridCell().SetPos(2, 2)); // 4
        cells.Add(new GameGridCell().SetPos(2, 3)); // 5

        cells.Add(new GameGridCell().SetPos(3, 1)); // 6
        cells.Add(new GameGridCell().SetPos(3, 2)); // 7
        cells.Add(new GameGridCell().SetPos(3, 3)); // 8

        // WHO LOVES SOOO MUCH WASTED MEMORY REWRITES>?
		UpdateGameGrid();
        UpdateCellPositions();
    }

	public void UpdateGameGrid()
    {
        dynaRoom.width = gridCellSize * 3;
        dynaRoom.length = gridCellSize * 3;
    }

	public void UpdateCellPositions()
    {
        for(int i = 0; i < cells.Count; i++)
        {
            GameGridCell cell = cells[i];
            GameGridCell newCell = cell;

            float halfGrid = (gridCellSize / 2f);

            float x = ((cell.x) * gridCellSize) - halfGrid;
            float y = ((cell.y) * gridCellSize) - halfGrid;

            float offset = (gridCellSize * 3f) / 2f;

            newCell.cellPos = new Vector3( y - offset, 0f, -(x - offset ));

            cells[i] = newCell;
        }
    }

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
			return (cells[GetPosInt(x, y)].player == go || cells[GetPosInt(x, y)].player == null);
	}

	public bool SetPosition(GameObject playerO, int x, int y)
	{
		if(!CheckPosition(x, y, playerO))
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

		player.RpcUpdatePosition(cell.cellPos);

		RpcWasGridUpdate();

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

	// Client-Side Commands (Run on Client's Instance)

    [ClientRpc]
    public void RpcUpdateGameGrid()
    {
        UpdateGameGrid();
    }

    [ClientRpc]
    public void RpcWasGridUpdate()
    {
        gridSelector.UpdateButtonStat();
    }
}

public partial class DC_GameGrid 
{
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

	[System.Serializable]
	public enum GridSize 
	{
		One,
		Two,
		Three
	}
}