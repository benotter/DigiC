using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_GridSelector : MonoBehaviour 
{
	public DC_HomeRoom homeRoom;
	public DC_GameGrid gameGrid;
	
	[Space(10)]
	public DC_GridSelector_Button[] buttons = new DC_GridSelector_Button[9];

	void Start () 
	{
		
	}
	
	void Update () 
	{
		
	}

	public void UpdateButtonStat()
	{
		foreach(DC_GridSelector_Button button in buttons)
			button.SetToolEnabled(!gameGrid.CheckPosition(button.xPos, button.yPos));
	}

	public void SetPlayerPositionOnGrid(int x, int y)
	{
		if(homeRoom.remotePlayer)
			homeRoom.remotePlayer.RequestSetPosition(x, y);
	}
}
