using System.Collections;
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

	public DC_LocalPlayer localPlayer;
    public GameObject remoteDisplay;

    [Space(10)]

    public DC_Game serverGame;
    public DC_Avatar avatar; 
    public DC_Avatar_Spawn avatarSpawn;

	[Space(10)]

    // Remote Variables (Set on the Server, Synced to the Clients)

    [SyncVar] public GameObject serverGameO = null;
	[SyncVar] public GameObject avatarO = null;
	[SyncVar] public GameObject avatarSpawnO = null;

    [Space(10)]

    [SyncVar] public string playerName = "";
    [SyncVar] public DC_Game.Teams currentTeam = DC_Game.Teams.None;
    
    [Space(10)]

    [SyncVar] public int gameGridX = 0;
	[SyncVar] public int gameGridY = 0;

    [Space(10)]

    [SyncVar] public int currentScore = 0;

    // Basically equilivent to 'Deaths' in any other game
    [SyncVar] public int chordStrucked = 0;
    

    

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
        if(serverGameO && !serverGame)
            serverGame = serverGameO.GetComponent<DC_Game>();

        if(avatarO && !avatar)
            avatar = avatarO.GetComponent<DC_Avatar>();

        if(avatarSpawnO && !avatarSpawn)
            avatarSpawn = avatarSpawnO.GetComponent<DC_Avatar_Spawn>();

        ServerUpdate();
        ClientUpdate();
	}

    [Server] public void ServerUpdate()
    {

    }

    [Client] public void ClientUpdate() 
    {

    }

    [Server] public void ClearAvatar() 
    {
        Debug.Log("Clearing Avatar!");
        
        RpcClearAvatar();

        if(avatarO)
            NetworkServer.Destroy(avatarO);
    }

    // Server-Side Commands (Run on Server's Instance of Object)
  
    [Command] public void CmdRequestAvatarSpawn()
    {
        serverGame.RequestAvatarSpawn(this);
    }

    [Command] public void CmdRequestAvatar()
    {
        serverGame.RequestAvatar(this);
    }

    [Command] public void CmdSetGridPosition(int x, int y)
    {
        if(avatarSpawn && avatarSpawn.lockedIn)
            return;

        gameGrid.SetPlayerToPosition(this.gameObject, x, y);
    }

    [Command] public void CmdSetPlayerName(string name)
    {
        playerName = name;
    }

    // Client-Side Commands (Run on Client's Instance of Object)

    [ClientRpc] public void RpcGameSetup(GameObject serverGO)
    {
        DC_Game sGame = serverGO.GetComponent<DC_Game>();

        serverGameO = serverGO;
        serverGame = sGame;

        gameGrid = sGame.gameGrid;
        
        if(hasAuthority)
        {
            localPlayer = sGame.localPlayer;
            homeRoom = sGame.homeRoom;
            
            homeRoom.SetRemotePlayer(this);
        }
    }

    [ClientRpc] public void RpcUpdatePosition(Vector3 pos)
    {
        transform.position = pos;

        if(hasAuthority)
            homeRoom.SetPosition(transform.position);
    }

    [ClientRpc] public void RpcSetAvatarSpawn(GameObject aS)
    {
        var avaS = aS.GetComponent<DC_Avatar_Spawn>();
        avatarSpawn = avaS;
    
        if(hasAuthority)
        {
            avaS.SetPlayer(gameObject);
            homeRoom.SetAvatarSpawn(avaS);
        }
    }

    [ClientRpc] public void RpcSetAvatar(GameObject a)
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

    [ClientRpc] public void RpcClearAvatar() 
    {
        GameObject.Destroy(avatarO);

        homeRoom.SetAvatar(null);
        avatar = null;
        avatarO = null;
    }
}
