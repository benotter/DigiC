using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_Game : NetworkBehaviour 
{
    public enum Team 
    {
        None,
        RedTeam,
        BlueTeam,
        CPUTeam,
        OtterCo,
    }
    // Client Variables

    public DC_HomeRoom homeRoom;
    public DC_GameGrid gameGrid;
    public DC_LocalPlayer localPlayer;

    [Space(10)]

    public GameObject avatarPrefab;
    public GameObject avatarSpawnPrefab;

    [Space(10)]

    public HashSet<GameObject> players = new HashSet<GameObject>();

    [HideInInspector] public NetworkClient gameOwner;

    // Server Variables
    [Space(10)]

    [SyncVar] public GameObject gameOwnerPlayerObj;

    [SyncVar] public string gameName = "";
    [SyncVar] public string gameAddress = "";
    [SyncVar] public int gamePort = 0;

    [Space(10)]

    [SyncVar] public int gameMaxPlayers = 8;
    [SyncVar] public float gameGridSize = 20;
    
    
    [Space(10)]
    [SyncVar] public int gamePlayerCount = 0;

    [SyncVar] public DC_Game_Round currentRound;
    [SyncVar] public DC_Game_ScoreCard scoreCard;


    // Private Client Variables
    private bool setupGameRoomSinceLastRound = false;

    void RegisterWithDirector()
    {
        GameObject director = GameObject.Find("DC_Director");
        DC_Director dc_D;

        if(director && (dc_D = director.GetComponent<DC_Director>()))
            dc_D.RegisterServerGame(this);
    }

    public override void OnStartServer()
    {
        RegisterWithDirector();
        SetGameSize(gameGridSize);
    }

    public override void OnStartClient()
    {
        RegisterWithDirector();
    }

    public void SetGameSize(float size)
    {
        gameGridSize = size;
        gameGrid.gridCellSize = size;

        gameGrid.UpdateDynaRoom();
        gameGrid.UpdateCellPositions();

        gameGrid.RpcUpdateGameGrid();
    }

    public void AddPlayer(DC_Player player)
    {
        if(player.connectionToClient.address == "localClient")
            gameOwnerPlayerObj = player.gameObject;

        player.playerName = player.connectionToClient.address;

        // Whhooooh, Lad.
        bool breaker = false;
        for(int x = 1; x <= 3; x++)
            if(!breaker) 
                for(int y = 1; y <= 3; y++)                
                    if(gameGrid.CheckPosition(x, y))
                        breaker = gameGrid.SetPlayerToPosition(player.gameObject, x, y);
            else
                break;

        gamePlayerCount++;
        RpcPlayerJoined(player.gameObject);
    }

    public void RemPlayer(DC_Player player)
    {
        // Player gridCell is auto-cleared by handy - dandy unity null referencing
        gamePlayerCount--;
        RpcPlayerLeft(player.gameObject);
    }

    public void RequestAvatarSpawn(DC_Player player)
    {
        Debug.Log("AvatarSpawn Request from " + player.playerName);

        if(!player.avatarSpawnO)
            return;

        GameObject avatarSpawnO = Instantiate(avatarSpawnPrefab);            
        DC_Avatar_Spawn avatarSpawn = avatarSpawnO.GetComponent<DC_Avatar_Spawn>();

        avatarSpawn.playerO = player.gameObject;
        player.avatarSpawnO = avatarSpawnO;

        player.avatarSpawn = avatarSpawn;

        if(NetworkServer.SpawnWithClientAuthority(avatarSpawnO, player.gameObject))
            player.RpcSetAvatarSpawn(avatarSpawnO);
        else
            NetworkServer.Destroy(avatarSpawnO);
    }
    
    public void RequestAvatar(DC_Player player)
    {
        Debug.Log("Avatar Request from " + player.playerName);

        if(!player.avatarSpawnO || player.avatarO)
            return;
        
        var aS = player.avatarSpawnO.GetComponent<DC_Avatar_Spawn>();

        if(!aS.lockedIn)
            return;

        GameObject avatarO = Instantiate(avatarPrefab);
        DC_Avatar avatar = avatarO.GetComponent<DC_Avatar>();

        avatar.playerO = player.gameObject;
        aS.PositionAvatar(avatarO);

        if(NetworkServer.SpawnWithClientAuthority(avatarO, player.gameObject))
        {
            player.avatarO = avatarO;
            player.avatar = avatar;
            player.RpcSetAvatar(avatarO);
        }
        else
            NetworkServer.Destroy(avatarO);
    }

    public void RequestPoints(DC_Player player, DC_Game_ScoreCard.PointType type)
    {
        int scoreAdd = 0;
        switch(type)
        {
            case DC_Game_ScoreCard.PointType.Chord:
                scoreAdd = scoreCard.chord;
            break;

            case DC_Game_ScoreCard.PointType.Goal:
                scoreAdd = scoreCard.goal;
            break;
        }

        player.currentScore += scoreAdd;
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


}

[System.Serializable]
public struct DC_Game_Round
{
    public enum GameType
    {
        FFA, // Free-For-All
        TVT, // Team-VS-Team
        TVC // Team-VS-Computer
    }

    public float startTime;
    public float duration;

    public float timeLeft;

    public int redScore;
    public int blueScore;

    public GameObject topPlayer;
    public int topPlayerScore;
}

[System.Serializable]
public class DC_Game_ScoreCard 
{
    public enum PointType
    {
        Goal,
        Chord,
    }

    public int goal = 150;
    public int chord = 25;
}