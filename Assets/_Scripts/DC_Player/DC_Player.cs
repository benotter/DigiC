using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_Player : NetworkBehaviour
{
	public DC_GameGrid gameGrid;
	public DC_HomeRoom homeRoom;

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
}
