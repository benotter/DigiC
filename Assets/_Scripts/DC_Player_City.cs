using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DC_Player_City : NetworkBehaviour 
{
	[Space(10)]

	[HideInInspector] public Vector3 northStreet;
	[HideInInspector] public Vector3 eastStreet;
	[HideInInspector]  public Vector3 southStreet;
	[HideInInspector] public Vector3 westStreet;

	// Server-Side Variables

	[SyncVar] public GameObject playerO;

	// Private Client-Side Variables

	private DC_Player player;

	void Update()
	{
		ServerUpdate();
		ClientUpdate();

		if(playerO && !player)
			player = playerO.GetComponent<DC_Player>();
	}

	[Server] public void ServerUpdate() 
	{

	}

	[Client] public void ClientUpdate() 
	{

	}
}
