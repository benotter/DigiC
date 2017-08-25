using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_GameGrid : NetworkBehaviour
{
	// Client-Side Variables
	public DynaRoom dynaRoom;
	public DC_GridSelector gridSelector;

	[HideInInspector] public bool gridSelectorDirty = false;

	// Server-Side Variables
	[SyncVar] public GridSize gridSize = GridSize.Three;
	[SyncVar] public float gridCellSize = 20;

	public SyncListGCell cells = new SyncListGCell();

	// Private Server-Side Variables

	// Private Client-Side Variables

	public override void OnStartClient()
    {
        UpdateDynaRoom();
    }

	public void UpdateDynaRoom()
    {
        dynaRoom.width = gridCellSize * 3;
        dynaRoom.length = gridCellSize * 3;
    }

	public bool CheckPosition(int x, int y, GameObject pO = null)
	{
		var pos = GetPosInt(x, y);

		if( pos >= cells.Count)
		{
			Debug.Log("Bad Grid Pos X: " + x);
			Debug.Log("Bad Grid Pos Y: " + y);
			return false;
		}
			
		var p = cells[pos].player;
		return (p == pO);
	}

	[Server] public bool SetPlayerToPosition(GameObject playerO, int x, int y)
	{
		if(!CheckPosition(x, y))
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

		GCell cell = cells[newPos];
		cell.player = playerO;
		cells[newPos] = cell;

		cells.Dirty(newPos);

		player.RpcUpdatePosition(cell.cellPos);

		RpcUpdateGameGrid();

		return true;
	}

	[Server] public void SetGridSize(GridSize sizeType, float cellSize)
	{
		gridSize = sizeType;

		UpdateCells();
		SetGridCellSize(cellSize);
	}

	[Server] public void SetGridCellSize(float size)
	{
		gridCellSize = size;

		UpdateCellPositions();
		UpdateDynaRoom();
	}

	[Server] public void UpdateCellPositions()
    {
        for(int i = 0; i < cells.Count; i++)
        {
            GCell cell = cells[i];
            GCell newCell = cell;

            float halfGrid = (gridCellSize / 2f);

            float x = ((cell.x) * gridCellSize) - halfGrid;
            float y = ((cell.y) * gridCellSize) - halfGrid;

            float offset = (gridCellSize * 3f) / 2f;

            newCell.cellPos = new Vector3( y - offset, 0f, -(x - offset ));

            cells[i] = newCell;

			cells.Dirty(i);
        }
    }

	[Server] public void UpdateCells()
	{
		cells.Clear();

		cells.Add(new GCell().SetPos(1, 1)); // 0
        cells.Add(new GCell().SetPos(1, 2)); // 1
        cells.Add(new GCell().SetPos(1, 3)); // 2
		
		if(gridSize == GridSize.One)
			return;

        cells.Add(new GCell().SetPos(2, 1)); // 3
        cells.Add(new GCell().SetPos(2, 2)); // 4
        cells.Add(new GCell().SetPos(2, 3)); // 5

		if(gridSize == GridSize.Two)
			return;

        cells.Add(new GCell().SetPos(3, 1)); // 6
        cells.Add(new GCell().SetPos(3, 2)); // 7
        cells.Add(new GCell().SetPos(3, 3)); // 8
	}

	public int GetPosInt(int x, int y)
	{
		return (((x - 1) * 3) + y) - 1;
	}

	// Client-Side Commands
    [ClientRpc] public void RpcUpdateGameGrid()
    {
        UpdateDynaRoom();

		gridSelectorDirty = true;
    }
}

public partial class DC_GameGrid 
{
	[System.Serializable] public enum GridSize 
	{
		One,
		Two,
		Three
	}

	[System.Serializable] public struct GCell 
	{
		public GameObject player;
		public Vector3 cellPos;
		public int x;
		public int y;
		public GCell SetPos(int x, int y)
		{
			this.x = x;
			this.y = y;

			return this;
		}
	}

	[System.Serializable] public class SyncListGCell : SyncListStruct<GCell>{}
}