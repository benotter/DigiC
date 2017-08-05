using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_Director : NetworkManager 
{
	public DC_Game game;

	void Start()
	{

	}

	void Update()
	{

	}

	public void StartGame(string gameName, int maxPlayers = 8)
	{
		var hostC = StartHost(connectionConfig, maxPlayers);

		if(hostC != null)
		{
			game.gameName = gameName;
			game.gameMaxPlayers = maxPlayers;
			game.gamePort = networkPort;
			game.gameAddress = networkAddress;

			game.gameOwner = hostC;
		}
	}

	public void JoinGame(string address, int port)
	{
		
	}
}
