using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_SpawnManager_SpawnAvatar : SubToolBase
{
	public DC_HomeRoom homeRoom;


	private bool requestedAvatar = false;

	public override void OnPress()
	{
		if(homeRoom.remotePlayer)
		{
			if(!requestedAvatar)
			{
				homeRoom.remotePlayer.CmdRequestAvatar();
				requestedAvatar = true;
			}
		}
			
	}
}
