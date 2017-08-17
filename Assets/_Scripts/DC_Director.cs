using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_Director : NetworkManager 
{
	public DC_Game game;

	[Space(10)]

	public DC_HomeRoom homeRoom;
	public DC_GameGrid gameGrid;

	void Start()
	{

	}

	void Update()
	{
        
	}

    // Server-Side Callbacks

	public override void OnStartHost() 
    {
        
    }

    public override void OnStartServer()
    {

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
        
        game.RemPlayer(player);

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
        {
            DC_Player player = playerO.GetComponent<DC_Player>();

            player.RpcSetGame(game.gameObject);
            game.RpcSetClientPlayer(playerO);

            game.AddPlayer(player);
        };
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

    // Client Hooks for in-game UI

	public void StartGame(string gameName, int maxPlayers = 8)
	{
		var hostC = StartHost(connectionConfig, maxPlayers);

		if(hostC != null)
		{
			game.gamePort = networkPort;
			game.gameAddress = networkAddress;

			game.gameName = gameName;
			game.gameMaxPlayers = maxPlayers;
			
			game.gameOwner = hostC;

            Debug.Log("Host Client Successfully Started!");
		}
	}

	public void JoinGame(string address, int port)
	{
		networkPort = port;
		networkAddress = address;
	}
}
