﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_Player : NetworkBehaviour
{
    // Local Variables (Set for Each Clients Specific Instances)

    public bool aLocalPlayer = false;

    [Space(10)]
	public DC_GameGrid gameGrid;
	public DC_HomeRoom homeRoom;

    

	[Space(10)]

	

	[Space(10)]

	public DC_LocalPlayer localPlayer;
    public GameObject remoteDisplay;

    [Space(10)]

    public DC_Game serverGame;
    public DC_Avatar avatar; 
    public DC_Avatar_Spawn avatarSpawn;

	[Space(10)]

    // Remote Variables (Set on the Server, Synced to the Clients)

    [SyncVar] public string playerName = "";
    
    [Space(10)]

    [SyncVar] public int gameGridX = 0;
	[SyncVar] public int gameGridY = 0;

    [Space(10)]

    [SyncVar] public int currentScore = 0;

    // Basically equilivent to 'Deaths' in any other game
    [SyncVar] public int chordStrucked = 0;
    [SyncVar] public DC_Game.Team currentTeam = DC_Game.Team.None;

	[Space(10)]
    
    [SyncVar] public GameObject serverGameO = null;
	[SyncVar] public GameObject avatarO = null;
	[SyncVar] public GameObject avatarSpawnO = null;
    

    // Private Client-Side Variables
    
	void Start () 
	{
		
	}

    public override void OnStartAuthority()
    {
        if(remoteDisplay)
            remoteDisplay.GetComponent<MeshRenderer>().enabled = false;

        aLocalPlayer = true;
    }
	
	void Update () 
	{
		if(isClient)
            UpdateClient();
        else if(isServer)
            UpdateServer();
	}

    public void UpdateServer()
    {

    }

    public void UpdateClient()
    {

    }

    // Server-Side Commands (Run on Server's Instance of Object)
    [Command]
    public void CmdSetPlayerName(string name)
    {
        playerName = name;
    }

    [Command]
    public void CmdRequestChord()
    {
        serverGame.RequestPoints(this, DC_Game_ScoreCard.PointType.Chord);
    }

    [Command]
    public void CmdRequestGoal()
    {
        serverGame.RequestPoints(this, DC_Game_ScoreCard.PointType.Goal);
    }

    [Command]
    public void CmdRequestAvatarSpawn()
    {
        serverGame.RequestAvatarSpawn(this);
    }

    [Command]
    public void CmdRequestAvatar()
    {
        serverGame.RequestAvatar(this);
    }

    [Command]
    public void CmdSetGridPosition(int x, int y)
    {
        if(avatarSpawnO)
        {
            var aS = avatarSpawnO.GetComponent<DC_Avatar_Spawn>();
            if(aS && aS.lockedIn)
                return;
        }
        
        if(!gameGrid.CheckPosition(x, y))
            gameGrid.SetPosition(this.gameObject, x, y);
    }

    // Client-Side Commands (Run on Client's Instance of Object)

    [ClientRpc]
    public void RpcSetGame(GameObject serverGameO)
    {
        DC_Game sGame = serverGameO.GetComponent<DC_Game>();

        gameGrid = sGame.gameGrid;
        serverGame = sGame;

        if(hasAuthority)
        {
            homeRoom = sGame.homeRoom;    
            localPlayer = sGame.localPlayer;
        }
    }

    [ClientRpc]
    public void RpcSetAvatarSpawn(GameObject aS)
    {
        var avaS = aS.GetComponent<DC_Avatar_Spawn>();
        avatarSpawn = avaS;
    
        if(hasAuthority)
        {
            avaS.SetPlayer(gameObject);
            homeRoom.SetAvatarSpawn(avaS);
        }
    }

    [ClientRpc]
    public void RpcSetAvatar(GameObject a)
    {
        var ava = a.GetComponent<DC_Avatar>();
        avatar = ava;

        if(hasAuthority)
        {
            ava.SetLocalPlayer(localPlayer);
            homeRoom.SetAvatar(ava);
            ava.UpdateBody();
        }
    }

    [ClientRpc]
    public void RpcUpdatePosition(Vector3 pos)
    {
        transform.position = pos;

        if(hasAuthority)
        {
            homeRoom.SetPosition(transform.position);
        }
    }
}
