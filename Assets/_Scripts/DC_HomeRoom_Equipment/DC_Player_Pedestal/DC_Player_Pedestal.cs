using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_Player_Pedestal : DC_HR_Equipment_Base 
{
	public GameObject point;

	public Vector3 playerPoint;

	public bool playerActive = false;

	void Start () 
	{
		TogglePlayerActive();
	}

	public void TogglePlayerActive(bool yes = false)
	{
		point.SetActive(playerActive = yes);
	}
	

	public override void OnJoinGame()
	{
		TogglePlayerActive(true);
	}

	public override void OnLeaveGame()
	{
		TogglePlayerActive(false);
	}
}
