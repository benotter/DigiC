using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_Avatar_Chest : DC_Avatar_Part 
{
	public Color playerColor = Color.blue;
	public Color damageColor = Color.red * new Color(1f,1f,1f,0.5f);

	public bool broken = false;


	private MeshRenderer rend;

	void Start () 
	{
				
	}
	
	// Update is called once per frame
	void Update () 
	{

	}
}
