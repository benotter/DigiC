using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DC_Player_City : NetworkBehaviour 
{

	// Server-Side Variables
	[SyncVar]
	public float gridWidth;

	[SyncVar]
	public float gridHeight;

	public Vector3 northStreet;
	public Vector3 eastStreet;
	public Vector3 southStreet;
	public Vector3 westStreet;
	

	void Start () 
	{
		
	}
	
	void Update () 
	{
		
	}
}
