using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_SpawnSelector : DC_HR_Equipment_Base 
{
	public DC_HomeRoom homeRoom;
	public DC_GameGrid gameGrid;

	[Space(10)]

	public DC_SpawnSelector_Handle handle;
	public DC_SpawnSelector_Marker marker;

	[Space(10)]

	public float padding = 2;

	[HideInInspector]
	public DC_Avatar_Spawn avatarSpawn;

	private Vector3 spawnPoint = Vector3.zero;

	void Update()
	{
		if(!marker.lockedIn)
		{
			float halfSize = gameGrid.gridCellSize / 2;
				  
			var p = homeRoom.transform.position;

			float newX = p.x + ( (halfSize - padding) * marker.xPos );
			float newZ = p.z + ( (halfSize - padding) * marker.zPos );

			spawnPoint = new Vector3(newX, gameGrid.transform.position.y + 0.3f, newZ);

			if(avatarSpawn)
				avatarSpawn.SetPosition(spawnPoint);
		}
	}


	public void LockedIn()
	{
		if(avatarSpawn)
		{
			avatarSpawn.SetPosition(spawnPoint);
			avatarSpawn.Lock();
		}
	}

	public void Unlocked()
	{
		if(avatarSpawn)
			avatarSpawn.Unlock();
	}

	public void SetAvatarSpawn(DC_Avatar_Spawn avaS = null) 
	{
		avatarSpawn = avaS;

		if(!avaS)
		{
			if(handle.inUse)
				handle.StopUse(handle.pTool);

			handle.toolEnabled = false;
		}
		else 
		{
			handle.toolEnabled = true;
		}
	}
}
