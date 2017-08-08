using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_GameGrid
{
    public override void OnStartServer()
    {
        cells = new SyncListGridCell() 
        {
            // Stupid Waste of memory because I'm bad at off-by-1 math
            new GameGridCell().SetPos(0, 0),

            new GameGridCell().SetPos(1, 1),
            new GameGridCell().SetPos(1, 2),
            new GameGridCell().SetPos(1, 3),

            new GameGridCell().SetPos(2, 1),
            new GameGridCell().SetPos(2, 2),
            new GameGridCell().SetPos(2, 3),

            new GameGridCell().SetPos(3, 1),
            new GameGridCell().SetPos(3, 2),
            new GameGridCell().SetPos(3, 3),
	    };           

        // WHO LOVES SOOO MUCH WASTED MEMORY REWRITES>?
        UpdateCellPositions();
        UpdateGameGrid();
    }

    public void UpdateCellPositions()
    {
        int pos = 0;
        foreach(GameGridCell cell in cells)
        {
            GameGridCell newCell = new GameGridCell();

            float x = (cell.x * gridCellSize) - (gridCellSize / 2);
            float y = (cell.y * gridCellSize) - (gridCellSize / 2);

            float offset = (gridCellSize * 3) / 2;

            newCell.cellPos = new Vector3(x - offset, 0f, y - offset);

            newCell.x = cell.x;
            newCell.y = cell.y;

            newCell.player = cell.player;

            cells[pos++] = newCell;
        }
    }
}
