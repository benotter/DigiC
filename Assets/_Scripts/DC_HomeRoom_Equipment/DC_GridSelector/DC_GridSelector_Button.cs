using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_GridSelector_Button : SubToolBase
{
	public DC_GridSelector gridSelector;

	public int xPos = 1;
	public int yPos = 1;

	public override void OnPress()
	{
		gridSelector.SetPlayerPositionOnGrid(xPos, yPos);
	}
}
