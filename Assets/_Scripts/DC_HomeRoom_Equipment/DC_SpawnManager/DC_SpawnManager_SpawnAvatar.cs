﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_SpawnManager_SpawnAvatar : SubToolBase
{
	public DC_HomeRoom homeRoom;
	
	public override void OnPress()
	{
		if(homeRoom.gameJoined)
			homeRoom.remotePlayer.CmdRequestAvatar();
	}
}
