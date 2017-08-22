using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_SpawnSelector_Handle : SubToolBase 
{
	public DC_SpawnSelector spawnSelector;

	[Space(10)]
	public DC_SpawnSelector_Marker marker;

	private float startY = 0f;

	private Quaternion startRot;

	private bool used = false;

	void Start()
	{
		startY = transform.position.y;
		startRot = transform.rotation;
	}

	void Update()
	{
		base.UpdateSubTool();

		if(inUse && !used)
			used = true;

		if(!inUse && used)
		{
			var newO = marker.transform.localPosition;
			newO.y = startY;

			SetOrigin(newO, startRot);
			
			used = false;
		}

		if(!inUse)
			return;
		
		if(trigger == 1f)
			marker.UpdatePosition();

		if(touchClick && !marker.lockedIn)
			marker.Lock();
		else if(!touchClick && marker.lockedIn)
			marker.UnLock();
	}
}
