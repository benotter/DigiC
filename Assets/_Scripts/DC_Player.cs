using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_Player : NetworkBehaviour
{
	public DC_GameGrid gameGrid;
	public DC_HomeRoom homeRoom;

	[Space(10)]

	public DC_Game serverGame;

	[Space(10)]

	public DC_LocalPlayer localPlayer;
	
	[Space(10)]

	[SyncVar]
	public GameObject serverGameObject = null;

	[SyncVar]
	public string playerName = "";

	[Space(10)]

	[SyncVar]
	public GameObject avatar = null;
	
	[SyncVar]
	public GameObject avatarSpawn = null;

	[Space(10)]

	[SyncVar]
	public int gameGridX = 0;

	[SyncVar]
	public int gameGridY = 0;

	void Start () 
	{
		
	}
	
	void Update () 
	{
		
	}

	public override void OnStartLocalPlayer()
    {


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
        if(avatarSpawn)
        {
            var aS = avatarSpawn.GetComponent<DC_Avatar_Spawn>();
            if(aS && aS.lockedIn)
                return;
        }
        
        if(!gameGrid.CheckPosition(x, y))
            gameGrid.SetPosition(this.gameObject, x, y);
    }

    [ClientRpc]
    public void RpcSetAvatarSpawn(GameObject aS)
    {
        avatarSpawn = aS;
        var avaS = aS.GetComponent<DC_Avatar_Spawn>();
        if(avaS)
            avaS.SetPlayer(gameObject);

        var p = transform.position;
        aS.transform.position = new Vector3(p.x, gameGrid.transform.position.y + 0.3f, p.z);
        avaS.Lock();
    }

    [ClientRpc]
    public void RpcSetAvatar(GameObject a)
    {
        avatar = a;
        var ava = a.GetComponent<DC_Avatar>();
        if(ava)
        {
            serverGame.homeRoom.avatarSync.SetAvatar(ava);
            ava.UpdateBody();
            a.transform.position = avatarSpawn.transform.position;
            
        }
    }
}
