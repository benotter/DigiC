using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_Player : NetworkBehaviour
{
	[SyncVar]
	public GameObject serverGameObject = null;

	[SyncVar]
	public string playerName = "";

	[Space(10)]

	[SyncVar]
	public GameObject avatar = null;

	void Start () 
	{
		
	}
	
	void Update () 
	{
		
	}
}
