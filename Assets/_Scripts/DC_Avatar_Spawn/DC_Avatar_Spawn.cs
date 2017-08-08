using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class DC_Avatar_Spawn : NetworkBehaviour
{
	[SyncVar]
	public GameObject playerO;

	[SyncVar]
	public bool lockedIn = false;

	private DC_Player player;
	void Start () 
	{
		
	}
	
	void Update () 
	{
		
	}

	public void SetPlayer(GameObject p)
	{
		playerO = p;
		player = playerO.GetComponent<DC_Player>();
	}

	public void Lock()
	{
		lockedIn = true;
	}

	public void Unlock()
	{
		lockedIn = false;
	}

	public void PositionAvatar(GameObject avatar)
	{
		avatar.transform.position = transform.position;
	}
}
