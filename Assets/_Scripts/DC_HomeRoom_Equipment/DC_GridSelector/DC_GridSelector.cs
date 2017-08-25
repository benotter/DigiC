using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_GridSelector : DC_HR_Equipment_Base 
{
	public DC_HomeRoom homeRoom;
	
	[Space(10)]
	
	public DC_GridSelector_Button[] buttons = new DC_GridSelector_Button[9];

	void Update() 
	{
		if(homeRoom.gameJoined && homeRoom.gameGrid.gridSelectorDirty)
		{
			
			homeRoom.gameGrid.gridSelectorDirty = false;
			UpdateButtonState();
		}
	}

	public void SetPlayerPositionOnGrid(int x, int y)
	{
		Debug.Log("Local Selecting Grid X: " + x);
		Debug.Log("Local Selecting Grid Y: " + y);

		if(homeRoom.gameJoined)
			homeRoom.remotePlayer.CmdSetGridPosition(x, y);
	}

	public void UpdateButtonState()
	{
		if(homeRoom.gameJoined)
		{
			foreach(DC_GridSelector_Button button in buttons)
			{
				bool en = homeRoom.gameGrid.CheckPosition(button.xPos, button.yPos);

				Debug.Log(homeRoom.gameGrid.cells[homeRoom.gameGrid.GetPosInt(button.xPos, button.yPos)].player);

				if(!en)
				{
					Debug.Log("Button Bad X: " + button.xPos);
					Debug.Log("Button Bad Y: " + button.yPos);
				}
				button.SetToolEnabled(en);
			}
		}
	}
}
