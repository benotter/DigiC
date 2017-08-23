﻿using System.Collections;
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

    public DC_Player remotePlayer;
    public GameObject remotePlayerO;

    [Space(10)]

    public GameObject avatarPrefab;
    public GameObject avatarSpawnPrefab;

    // Server Variables

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

    [SyncVar]
    public DC_Game_Round currentRound;

    [SyncVar]
    public DC_Game_ScoreCard scoreCard;


    // Private Client Variables
    private bool setupGameRoomSinceLastRound = false;


    public override void OnStartServer()
    {
        gameGrid.gridCellSize = gameGridSize;
    }

    public void SetGameSize(float size)
    {
        gameGridSize = size;
        gameGrid.gridCellSize = size;

        gameGrid.UpdateGameGrid();
        gameGrid.UpdateCellPositions();

        gameGrid.RpcUpdateGameGrid();
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

    public void RequestAvatarSpawn(DC_Player player)
    {
        Debug.Log("AvatarSpawn Request from " + player.playerName);

        if(player && !player.avatarSpawnO)
        {
            GameObject avatarSpawnO = Instantiate(avatarSpawnPrefab);
            NetworkServer.SpawnWithClientAuthority(avatarSpawnO, player.gameObject);
            DC_Avatar_Spawn avatarSpawn = avatarSpawnO.GetComponent<DC_Avatar_Spawn>();

            avatarSpawn.playerO = player.gameObject;
            player.avatarSpawnO = avatarSpawnO;

            player.avatarSpawn = avatarSpawn;
            
            player.RpcSetAvatarSpawn(avatarSpawnO);
        }
    }
    
    public void RequestAvatar(DC_Player player)
    {
        Debug.Log("Avatar Request from " + player.playerName);

        if( player.avatarSpawnO && !player.avatarO)
        {
            var aS = player.avatarSpawnO.GetComponent<DC_Avatar_Spawn>();

            if(aS && aS.lockedIn)
            {
                GameObject avatarO = Instantiate(avatarPrefab);

                if(!avatarO || !player || !player.gameObject)
                {
                    Debug.Log("Fucked up Spawn!");
                    return;
                }
                    
                NetworkServer.SpawnWithClientAuthority(avatarO, player.gameObject);

                if(!avatarO)
                    return;
                    
                DC_Avatar avatar = avatarO.GetComponent<DC_Avatar>();

                avatar.playerO = player.gameObject;
                player.avatarO = avatarO;

                player.avatar = avatar;
            
                aS.PositionAvatar(avatarO);
                player.RpcSetAvatar(avatarO);
            }
        }
    }

    public void AddPlayer(DC_Player player)
    {
        player.serverGameObject = gameObject;
        player.serverGame = this;

        player.gameGrid = gameGrid;

        player.playerName = player.connectionToClient.address;

        if(player.connectionToClient.address == "localClient")
            gameOwnerPlayerObj = player.gameObject;
        
        RpcPlayerJoined(player.gameObject);
        gamePlayerCount++;

        bool breaker = false;

        for(int x = 1; x <= 3; x++)
        {
            for(int y = 1; y <= 3; y++)
            {
                if(!gameGrid.CheckPosition(x, y))
                {
                    gameGrid.SetPosition(player.gameObject, x, y);
                    breaker = true;
                    break;
                }
            }

            if(breaker)
                break;
        }
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

    [ClientRpc]
    public void RpcSetClientPlayer(GameObject playerO)
    {
        DC_Player player = playerO.GetComponent<DC_Player>();

        if(player && player.hasAuthority)
        {
            remotePlayerO = playerO;
            remotePlayer = player;

            homeRoom.SetRemotePlayer(remotePlayer);
        }
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