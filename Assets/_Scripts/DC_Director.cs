﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_Director : NetworkManager 
{
    [Space(10)]
    public string gameName = "Test Game";
    public int maxPlayers = 8;

    [Space(10)]

    public DC_HomeRoom homeRoom;

    [Space(10)]

    public GameObject serverGamePrefab;

    [Space(10)]

    public DC_Game serverGame;

    [HideInInspector] public GameObject serverGameO;

    private bool gameSpawned = false;

    void Update() 
    {
        if(NetworkServer.active && !gameSpawned) 
        {
            NetworkServer.Spawn(serverGameO);
            gameSpawned = true;
        }
    }

    public void RegisterServerGame(DC_Game server)
    {
        if(!serverGame)
        {
            serverGame = server;
            serverGameO = server.gameObject;
        }
    
        server.homeRoom = homeRoom;
        homeRoom.serverGame = server;

        server.localPlayer = homeRoom.localPlayer;
        
        if(server.gameGridO && !server.gameGrid)
            server.gameGrid = server.gameGridO.GetComponent<DC_GameGrid>();
            
        homeRoom.gameGrid = server.gameGrid;
    }

    public void CreateServerGame()
    {
        serverGameO = Instantiate(serverGamePrefab);
        serverGame = serverGameO.GetComponent<DC_Game>();

        serverGame.gameName = gameName;
        serverGame.gameMaxPlayers = maxPlayers;
        serverGame.gamePort = networkPort;
        serverGame.gameAddress = networkAddress;
    }

    void Start() 
    {
        
    }

    // Server-Side Callbacks
	public override void OnStartHost() 
    {
        CreateServerGame();
    }

    public override void OnStartServer()
    {
        if(!serverGameO)
            CreateServerGame();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log("Client Join From: " + conn.address);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Debug.Log("Client Left From: " + conn.address);

        GameObject playerO = conn.playerControllers[0].gameObject;
        DC_Player player = playerO.GetComponent<DC_Player>();
        
        serverGame.RemPlayer(player);

        if(player.avatarSpawnO)
            NetworkServer.Destroy(player.avatarSpawnO);

        if(player.avatarO)
            NetworkServer.Destroy(player.avatarO);

        NetworkServer.DestroyPlayersForConnection(conn);
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        Debug.Log("Client Ready On: " + conn.address);
        NetworkServer.SetClientReady(conn);
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        Debug.Log("Client Adding Player On: " + conn.address);

        GameObject playerO = Instantiate(playerPrefab, Vector3.zero, Quaternion.Euler(Vector3.zero));

        if( NetworkServer.AddPlayerForConnection(conn, playerO, playerControllerId) )
            serverGame.AddPlayer(playerO);
    }
    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
        Debug.Log("Client Removing Player On: " + conn.address);
        base.OnServerRemovePlayer(conn, player);
    }

    // Client-Side Callbacks
	public override void OnClientConnect(NetworkConnection conn)
    {
        ClientScene.Ready(conn);

        Debug.Log("Connected to " + conn.address);

        ClientScene.AddPlayer(0);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        StopClient();
    }

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {

    }

    public override void OnClientNotReady(NetworkConnection conn)
    {

    }

	public void StartGame()
	{
		var hostC = StartHost();

		if(hostC != null)
		{
			serverGame.gameOwner = hostC;
            Debug.Log("Host Client Successfully Started!");
		}
	}

	public void JoinGame(string address, int port)
	{
		networkPort = port;
		networkAddress = address;
	}
}
