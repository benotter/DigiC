using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_Player : NetworkBehaviour
{
    // Local Variables (Set for Each Clients Specific Instances)
	public DC_GameGrid gameGrid;
	public DC_HomeRoom homeRoom;

	[Space(10)]

	public DC_Game serverGame;

	[Space(10)]

	public DC_LocalPlayer localPlayer;

    public GameObject remoteDisplay;

    [Space(10)]

    public DC_Avatar avatar; 
    public DC_Avatar_Spawn avatarSpawn;
	
	[Space(10)]

    // Remote Variables (Set on the Server, Synced to the Clients)

	[SyncVar]
	public GameObject serverGameObject = null;

	[SyncVar]
	public string playerName = "";

	[Space(10)]

	[SyncVar]
	public GameObject avatarO = null;
	
	[SyncVar]
	public GameObject avatarSpawnO = null;

	[Space(10)]

	[SyncVar]
	public int gameGridX = 0;

	[SyncVar]
	public int gameGridY = 0;

    // Private Client-Side Variables

    private bool posHomeRoomDirty = false;
    private bool posAvatarSpawnDirty = false;


	void Start () 
	{
		
	}

    public override void OnStartAuthority()
    {
        if(remoteDisplay)
            remoteDisplay.SetActive(false);
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
        if(hasAuthority)
        {
            if(posHomeRoomDirty && homeRoom)
            {
                homeRoom.SetPosition(transform.position);

                posHomeRoomDirty = false;
            }
            
            if(posAvatarSpawnDirty && avatarSpawn && gameGrid)
            {
                var p = transform.position;
                avatarSpawn.SetPosition(new Vector3(p.x, gameGrid.transform.position.y + 0.3f, p.z));
                avatarSpawn.Lock();

                posAvatarSpawnDirty = false;
            }
        }
    }

    // Server-Side Commands (Run on Server's Instance of Object)

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
        if(hasAuthority)
        {
            DC_Game sGame = serverGameO.GetComponent<DC_Game>();

            homeRoom = sGame.homeRoom;
            gameGrid = sGame.gameGrid;
            localPlayer = sGame.localPlayer;
        }
    }

    [ClientRpc]
    public void RpcSetAvatarSpawn(GameObject aS)
    {
        var avaS = aS.GetComponent<DC_Avatar_Spawn>();

        avatarSpawn = avaS;
        avaS.SetPlayer(gameObject);
        posAvatarSpawnDirty = true;
    }

    [ClientRpc]
    public void RpcSetAvatar(GameObject a)
    {
        var ava = a.GetComponent<DC_Avatar>();

        ava.SetLocalPlayer(localPlayer);

        avatar = ava;
        homeRoom.SetAvatar(ava);
        
        ava.UpdateBody();
    }

    [ClientRpc]
    public void RpcSetPosition(Vector3 pos)
    {
        transform.position = pos;

        if(hasAuthority)
            posHomeRoomDirty = true;
    }
}
