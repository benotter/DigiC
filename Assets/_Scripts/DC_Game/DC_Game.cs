using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_Game : NetworkBehaviour 
{
    public DC_HomeRoom homeRoom;
    public DC_GameGrid gameGrid;

    [Space(10)]

    public GameObject remotePlayer;

    [Space(10)]

    public GameObject avatarPrefab;
    public GameObject avatarSpawnPrefab;

    [Space(10)]

    public HashSet<GameObject> players = new HashSet<GameObject>();

    [Space(10)]
    

    [SyncVar]
	public string gameName = "";

    [SyncVar]
    public int gameMaxPlayers = 8;

    [SyncVar]
    public string gameAddress = "";

    [SyncVar]
    public int gamePort = 0;

    [SyncVar]
    public int gamePlayerCount = 0;

    [SyncVar]
    public GameObject gameOwnerPlayerObj;

    [SyncVar]
    public float gameGridSize = 20;

    void Start()
    {

    }
}

public partial class DC_Game 
{
    public void SetGameSize(float size)
    {
        gameGridSize = size;
        gameGrid.gridCellSize = size;

        
        gameGrid.UpdateCellPositions();
        
        gameGrid.UpdateGameGrid();
        gameGrid.RpcUpdateGameGrid();
    }
}
