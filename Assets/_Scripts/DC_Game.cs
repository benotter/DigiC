using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_Game : NetworkBehaviour 
{

    // Client Variables

    [Header("Game Prefabs")]

    public GameObject gameRoundPrefab;

    [Space(10)]

    public GameObject gameGridPrefab;

    [Space(10)]

    public GameObject avatarPrefab;
    public GameObject avatarSpawnPrefab;

    [Space(10)]

    public GameObject playerCityPrefab;

    [Space(10)] [Header("Local Info")]

    public DC_LocalPlayer localPlayer;

    [Space(10)] [Header("Remote Info")]    

    public DC_HomeRoom homeRoom;
    public DC_GameGrid gameGrid;

    [Space(10)]

    public HashSet<GameObject> players = new HashSet<GameObject>();

    [HideInInspector] public HashSet<GameObject> preStartPlayers = new HashSet<GameObject>();
    [HideInInspector] public NetworkClient gameOwner;

    // Server Variables
    [Space(10)]

    [SyncVar] public GameObject gameGridO;

    [Space(10)]

    [SyncVar] public GameObject gameOwnerPlayerObj;

    [Space(10)]

    [SyncVar] public int gamePlayerCount = 0;

    [Space(10)]

    [SyncVar] public GameObject currentRound;

    [Space(10)]

    [SyncVar] public string gameName = "";
    [SyncVar] public string gameAddress = "";
    [SyncVar] public int gamePort = 0;

    [Space(10)]

    [SyncVar] public int gameMinPlayers = 2;
    [SyncVar] public int gameMaxPlayers = 8;

    [Space(10)]

    [SyncVar] public DC_GameGrid.GridSize gameGridSize = DC_GameGrid.GridSize.Three;
    [SyncVar] public float gameGridCellSize = 100;

    // Private Client Variables

    [Client] void RegisterWithDirector()
    {
        GameObject director = GameObject.Find("DC_Director");
        DC_Director dc_D;

        if(director && (dc_D = director.GetComponent<DC_Director>()))
            dc_D.RegisterServerGame(this);
    }

    public override void OnStartServer()
    {
        CreateGameGrid();

        foreach(GameObject playerO in preStartPlayers)
            AddPlayer(playerO);
    }

    public override void OnStartClient()
    {
        RegisterWithDirector();
    }

    [Server] public void SetGameSize(DC_GameGrid.GridSize gSize, float size)
    {
        gameGridSize = gSize;
        gameGridCellSize = size;

        gameGrid.SetGridSize(gSize, size);
    }

    [Server] void CreateGameGrid()
    {
        var go = Instantiate(gameGridPrefab);
        
        NetworkServer.Spawn(go);

        gameGridO = go;
        gameGrid = go.GetComponent<DC_GameGrid>();

        gameGrid.SetGridSize(gameGridSize, gameGridCellSize);
    }

    [Server] public void AddPlayer(GameObject playerO)
    {
        if(!gameGrid)
        {
            preStartPlayers.Add(playerO);
            return;
        }

        DC_Player player = playerO.GetComponent<DC_Player>();

        player.RpcGameSetup(gameObject);

        if(player.connectionToClient.address == "localClient")
            gameOwnerPlayerObj = player.gameObject;

        // Whhooooh, Lad.
        bool breaker = false;
        for(int x = 1; x <= 3; x++)
        {
            for(int y = 1; y <= 3; y++)
            {
                if(gameGrid.CheckPosition(x, y))
                    breaker = gameGrid.SetPlayerToPosition(player.gameObject, x, y);
                else 
                {
                    Debug.Log("Bad X pos: " + x);
                    Debug.Log("Bad Y pos: " + y);
                }

                if(breaker)
                    break;
            }
            if(breaker)
                break;
        }
            

        gamePlayerCount++;
        RpcPlayerJoined(player.gameObject);

        gameGrid.UpdateAllClients();
    }

    [Server] public void RemPlayer(DC_Player player)
    {
        // Player gridCell is auto-cleared by handy - dandy unity null referencing
        gamePlayerCount--;
        RpcPlayerLeft(player.gameObject);

        gameGrid.UpdateAllClients();
    }

    [Server] public void RequestAvatarSpawn(DC_Player player)
    {
        Debug.Log("AvatarSpawn Request from " + player.playerName);

        if(player.avatarSpawnO)
            return;

        GameObject avatarSpawnO = Instantiate(avatarSpawnPrefab);            
        DC_Avatar_Spawn avatarSpawn = avatarSpawnO.GetComponent<DC_Avatar_Spawn>();
        
        avatarSpawn.playerO = player.gameObject;

        if(NetworkServer.SpawnWithClientAuthority(avatarSpawnO, player.gameObject))
        {
            player.avatarSpawn = avatarSpawn; // Only on Server

            player.avatarSpawnO = avatarSpawnO;
            player.RpcSetAvatarSpawn(avatarSpawnO);
        }
        else
            NetworkServer.Destroy(avatarSpawnO);
    }
    
    [Server] public void RequestAvatar(DC_Player player)
    {
        Debug.Log("Avatar Request from " + player.playerName);

        if(!player.avatarSpawn || player.avatar)
            return;

        if(!player.avatarSpawn.lockedIn)
            return;

        GameObject avatarO = Instantiate(avatarPrefab);
        DC_Avatar avatar = avatarO.GetComponent<DC_Avatar>();

        avatar.playerO = player.gameObject;

        if(NetworkServer.SpawnWithClientAuthority(avatarO, player.gameObject))
        {
            player.avatar = avatar; // Only on Server

            player.avatarO = avatarO;
            player.RpcSetAvatar(avatarO);

            player.avatarSpawn.PositionAvatar(avatar);
        }
        else
            NetworkServer.Destroy(avatarO);
    }

    // Client-Side Commands
    
    [ClientRpc] public void RpcPlayerJoined(GameObject player)
    {
        if(!players.Contains(player))
            players.Add(player);
    }

    [ClientRpc] public void RpcPlayerLeft(GameObject player)
    {
        if(players.Contains(player))
            players.Remove(player);
    }

    [ClientRpc] public void RpcNewRound(GameObject gameRoundO) 
    {

    }
}

public partial class DC_Game 
{
    [System.Serializable] public enum Teams 
    {
        None,
        RedTeam,
        BlueTeam,
        CPUTeam,
        OtterCo,
    }
}