using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_Player_Pedestal : DC_HR_Equipment_Base 
{
	public GameObject point;

	void Start () 
	{
		TogglePlayerActive();
	}

	public void TogglePlayerActive(bool active = false)
	{
		point.SetActive(active);
	}
	

	public override void OnJoinGame(DC_Player p)
	{
		TogglePlayerActive(true);
	}

	public override void OnLeaveGame()
	{
		TogglePlayerActive(false);
	}
}
