using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_SpawnManager_LockSpawn : SubToolBase 
{
	public DC_HomeRoom homeRoom;

	public override void OnPress()
	{
		if(homeRoom.remotePlayer)
		{
			homeRoom.remotePlayer.CmdRequestAvatarSpawn();
		}
	}
}
