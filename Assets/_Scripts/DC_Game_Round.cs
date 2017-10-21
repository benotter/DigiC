using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable] public class DC_Game_Round : NetworkBehaviour
{
	public enum GameType
	{
		FreeForAll,
		TeamVsTeam,
		TeamVSComputer
	}

	public float currentTime;

	[Space(10)]

	// Server-Side Variables

	[SyncVar] public bool roundStarted = false;
	[SyncVar] public bool roundInSetup = false;

	[Space(10)]

	[SyncVar] public float roundDuration = (5 * 60 * 1000);
	[SyncVar] public float roundSetupTime = (30 * 1000);
	
	[Space(10)]

	[HideInInspector] [SyncVar] public float roundStartTime = 0;
	[HideInInspector] [SyncVar] public float roundEndTime = 0;

	[SyncVar] public GameObject topPlayer;
	[SyncVar] public int topPlayerScore;

	[SyncVar] public int redScore;
	[SyncVar] public int blueScore;


	

	void Update()
	{
		ServerUpdate();
		ClientUpdate();
	}

	[Server] void ServerUpdate() 
	{
		if(roundInSetup || roundStarted)
			currentTime += Time.deltaTime;

		if(roundStarted && roundInSetup && currentTime > roundStartTime)
			StartRound();
		

	}

	[Client] void ClientUpdate() 
	{

	}

	public void StartRoundSetup() 
	{
		roundStartTime = roundSetupTime;
		roundEndTime = roundDuration + roundSetupTime;

		roundInSetup = true;
	}

	public void StartRound() 
	{
		roundInSetup = false;
	}

	// Client-Side Commands

	[ClientRpc] public void RpcRoundStarted() 
	{

	}

	[ClientRpc] public void RpcRoundEnded() 
	{

	}

	[ClientRpc] public void RpcUpdateTime() 
	{

	}
}
