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

    public NetworkClient gameOwner;

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


    private bool setupGameRoomSinceLastRound = false;

    public void SetGameSize(float size)
    {
        gameGridSize = size;
        gameGrid.gridCellSize = size;

        
        gameGrid.UpdateCellPositions();
        
        gameGrid.UpdateGameGrid();
        gameGrid.RpcUpdateGameGrid();
    }


    public override void OnStartServer()
    {
        gameGrid.gridCellSize = gameGridSize;
    }

    public void RequestAvatarSpawn(DC_Player player)
    {
        if(!player.avatarSpawn)
        {
            GameObject avatarSpawn = Instantiate(avatarSpawnPrefab);
            NetworkServer.SpawnWithClientAuthority(avatarSpawn, player.gameObject);

            player.RpcSetAvatarSpawn(avatarSpawn);
        }
    }
    
    public void RequestAvatar(DC_Player player)
    {
        if(!player.avatar && player.avatarSpawn)
        {
            var aS = player.avatarSpawn.GetComponent<DC_Avatar_Spawn>().lockedIn;

            if(aS)
            {
                GameObject avatar = Instantiate(avatarPrefab);
                NetworkServer.SpawnWithClientAuthority(avatar, player.gameObject);

                player.RpcSetAvatar(avatar);
            }
        }
    }

    public void AddPlayer(DC_Player player)
    {
        player.serverGameObject = gameObject;
        player.serverGame = this;

        if(player.gameObject == gameOwner.connection.playerControllers[0].gameObject)
            gameOwnerPlayerObj = player.gameObject;

        RpcPlayerJoined(player.gameObject);
        gamePlayerCount++;
    }

    public void RemPlayer(DC_Player player)
    {
        RpcPlayerLeft(player.gameObject);
        gamePlayerCount--;
    }

    [ClientRpc]
    public void RpcSetupGameRoom()
    {
        if(!setupGameRoomSinceLastRound)
        {
            setupGameRoomSinceLastRound = true;
            // gameRoom.SetupGameRoom();
        }
    }
    
    [ClientRpc]
    public void RpcPlayerJoined(GameObject player)
    {
        if(!players.Contains(player))
            players.Add(player);
    }

    [ClientRpc]
    public void RpcPlayerLeft(GameObject player)
    {
        if(players.Contains(player))
            players.Remove(player);
    }
}
