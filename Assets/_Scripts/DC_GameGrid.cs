using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_GameGrid : NetworkBehaviour
{
	// Client-Side Variables
	public DynaRoom dynaRoom;
	public DC_GridSelector gridSelector;

	// Server-Side Variables
	[SyncVar] public GridSize gridSize = GridSize.One;
	[SyncVar] public float gridCellSize = 20;
	[HideInInspector] public SyncListGridCell cells = new SyncListGridCell();

	// Private Client-Side Variables

	// Private Server-Side Variables

	void RegisterWithDirector()
	{
		GameObject director = GameObject.Find("DC_Director");
        DC_Director dc_D;

        if(director && (dc_D = director.GetComponent<DC_Director>()))
			dc_D.RegisterGameGrid(this);
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
    }

	public override void OnStartClient()
    {
        UpdateDynaRoom();
    }

	public void UpdateDynaRoom()
    {
        dynaRoom.width = gridCellSize * 3;
        dynaRoom.length = gridCellSize * 3;
    }

	[Server] public void UpdateCellPositions()
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

			cells.Dirty(i);
        }

		RpcUpdateGameGrid();
    }

	public int GetPosInt(int x, int y)
	{
		return (((x - 1) * 3) + y) - 1;
	}

	[Server] public bool CheckPosition(int x, int y, GameObject pO = null)
	{
		var pos = GetPosInt(x, y);

		if( pos >= cells.Count || pos < 0)
			return false;

		var p = cells[pos].player;
		return (p == pO);
	}

	[Server] public bool SetPlayerToPosition(GameObject playerO, int x, int y)
	{
		if(CheckPosition(x, y, playerO))
			return true;
		else if(!CheckPosition(x, y))
			return false;

		DC_Player player = playerO.GetComponent<DC_Player>();
		
		if(player.gameGridX > 0)
		{
			int oldPos = GetPosInt(player.gameGridX, player.gameGridY);
			var oldCell = cells[oldPos];
			oldCell.player = null;
			cells[oldPos] = oldCell;

			cells.Dirty(oldPos);
		}

		player.gameGridX = x;
		player.gameGridY = y;

		int newPos = GetPosInt(x, y);

		GameGridCell cell = cells[newPos];
		cell.player = playerO;
		cells[newPos] = cell;
		cells.Dirty(newPos);

		player.RpcUpdatePosition(cell.cellPos);

		RpcUpdateGameGrid();

		return true;
	}

	// Client-Side Commands

    [ClientRpc] public void RpcUpdateGameGrid()
    {
        UpdateDynaRoom();
		gridSelector.UpdateButtonState();
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