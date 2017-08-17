using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class DC_Avatar_Spawn : NetworkBehaviour
{
	// Server Variables

	[SyncVar]
	public GameObject playerO;

	[SyncVar]
	public bool lockedIn = false;


	// Private Client Variables
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

	[Command]
	public void CmdLock()
	{
		lockedIn = true;
	}

	[Command]
	public void CmdUnlock()
	{
		lockedIn = false;
	}

	// Client-Side Commands

	[ClientRpc]
	public void RpcSetPosition(Vector3 pos)
	{
		SetPosition(pos);
	}
}
