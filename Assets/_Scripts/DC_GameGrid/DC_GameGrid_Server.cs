using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_GameGrid
{
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
        UpdateCellPositions();
        UpdateGameGrid();
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
}
