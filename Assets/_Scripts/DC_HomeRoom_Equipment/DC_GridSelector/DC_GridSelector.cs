using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_GridSelector : DC_HR_Equipment_Base 
{
	public DC_HomeRoom homeRoom;
	public DC_GameGrid gameGrid;
	
	[Space(10)]
	
	public DC_GridSelector_Button[] buttons = new DC_GridSelector_Button[9];

	public void SetPlayerPositionOnGrid(int x, int y)
	{
		if(homeRoom.gameJoined)
			homeRoom.remotePlayer.CmdSetGridPosition(x, y);
	}

	public void UpdateButtonState()
	{
		if(homeRoom.gameJoined)
			foreach(DC_GridSelector_Button button in buttons)
				button.SetToolEnabled(gameGrid.CheckPosition(button.xPos, button.yPos));
	}
}
