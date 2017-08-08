using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_GameGrid
{
	public DynaRoom dynaRoom;

    public DC_GridSelector gridSelector;

    public override void OnStartClient()
    {
        UpdateGameGrid();
    }

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
