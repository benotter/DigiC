﻿using System.Collections;
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
