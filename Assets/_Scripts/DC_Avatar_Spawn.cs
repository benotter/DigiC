using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class DC_Avatar_Spawn : NetworkBehaviour
{
	// Server-Side Variables

	[SyncVar] public GameObject playerO;

	[SyncVar] public bool lockedIn = false;


	// Private Client-Side Variables
	private DC_Player player;

	public void SetPlayer(GameObject p)
	{
		player = p.GetComponent<DC_Player>();
	}

	public void Lock()
	{
		CmdLock();
	}

	public void Unlock()
	{
		CmdUnlock();
	}

	public void PositionAvatar(GameObject avatarO)
	{
		DC_Avatar avatar = avatarO.GetComponent<DC_Avatar>();
		PositionAvatar(avatar);
	}
	public void PositionAvatar(DC_Avatar avatar)
	{
		avatar.RpcSetPosition(transform.position);
	}

	public void SetPosition(Vector3 pos)
	{
		transform.position = pos;
	}

	// Server-Side Commands
	[Command] public void CmdLock()
	{
		lockedIn = true;
	}

	[Command] public void CmdUnlock()
	{
		lockedIn = false;
	}

	// Client-Side Commands
}
