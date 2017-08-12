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

	public override void OnStartHost() 
    {
        
    }

    public override void OnStartServer()
    {

    }

    public override void OnServerConnect(NetworkConnection conn)
    {

    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject playerO = Instantiate(playerPrefab, Vector3.zero, Quaternion.Euler(Vector3.zero));
        DC_Player player = playerO.GetComponent<DC_Player>();

        NetworkServer.AddPlayerForConnection(conn, playerO, playerControllerId);
        game.AddPlayer(player);
        game.RpcSetupGameRoom();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        GameObject playerO = conn.playerControllers[0].gameObject;
        DC_Player player = playerO.GetComponent<DC_Player>();
        
        game.RemPlayer(player);

        if(player.avatar)
            NetworkServer.Destroy(player.avatar);

        NetworkServer.Destroy(playerO);
    }

	public override void OnClientConnect(NetworkConnection conn)
    {
        ClientScene.Ready(conn);
        if(ClientScene.AddPlayer(0))
        {
            GameObject playerObj = ClientScene.localPlayers[0].gameObject;
            
            game.remotePlayer = playerObj;

            DC_Player player = playerObj.GetComponent<DC_Player>();
            if(player)
            {
                player.homeRoom = homeRoom;
                player.gameGrid = gameGrid;
                game.homeRoom.SetRemotePlayer(player);
            }
        }
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {

    }

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
		}
	}

	public void JoinGame(string address, int port)
	{
		networkPort = port;
		networkAddress = address;
	}
}
