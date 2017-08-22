using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_SpawnSelector : MonoBehaviour 
{
	public DC_HomeRoom homeRoom;
	public DC_GameGrid gameGrid;

	[Space(10)]

	public DC_SpawnSelector_Handle handle;
	public DC_SpawnSelector_Marker marker;

	private DC_Avatar_Spawn avatarSpawn;

	private Vector3 spawnPoint = Vector3.zero;

	void Update()
	{
		if(!marker.lockedIn)
		{
			var p = homeRoom.transform.position;
			spawnPoint = new Vector3(p.x, gameGrid.transform.position.y + 0.3f, p.z);

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

		if(avaS)
		{
			handle.toolEnabled = true;
		}
		else 
		{
			if(handle.inUse)
				handle.StopUse(handle.pTool);

			handle.toolEnabled = false;
		}
	}
}
