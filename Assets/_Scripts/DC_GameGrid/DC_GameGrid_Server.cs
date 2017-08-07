using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_GameGrid
{
    public override void OnStartServer()
    {
        for(int x = 1; x <= 3; x++)
            for(int y = 1; y <= 3; y++)
            {
                var cell = new GameGridCell();
                cell.x = x;
                cell.y = y;
                cells.Add(cell);
            }
                
    }
}
