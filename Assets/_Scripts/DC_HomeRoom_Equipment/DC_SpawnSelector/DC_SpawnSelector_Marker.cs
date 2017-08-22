using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_SpawnSelector_Marker : MonoBehaviour 
{
	public DC_SpawnSelector spawnSelector;

	[Space(10)]

	public DC_SpawnSelector_Handle handle;
	public GameObject markerPoint;

	[Space(10)]

	public float xSize = 0.15f;
	public float zSize = 0.15f;

	[Space(10)]

	public float spinRate = 0.01f;
	public float lockDownDist = 0.2f;

	[Space(10)]

	public Material laserUnlocked;
	public Material laserLocked;


	[HideInInspector]
	public bool lockedIn = false;


	private LineRenderer lineR;
	private Vector3 startPos;
	
	void Start() 
	{
		lineR = GetComponent<LineRenderer>();

		startPos = transform.position;
	}

	void Update()
	{
		if(!lockedIn)
		{
			var rot = transform.eulerAngles;
			rot.y += spinRate * Time.deltaTime;
			transform.rotation = Quaternion.Euler(rot);
		}
	}

	public void UpdatePosition()
	{
		if(lockedIn)
			return;

		var newPos = markerPoint.transform.position;

		var off = newPos - startPos;

		if(off.x > xSize || off.x < -xSize)
			newPos.x = startPos.x + ( off.x > 0 ? xSize : -xSize );

		if(off.z > zSize || off.z < -zSize)
			newPos.z = startPos.z + ( off.z > 0 ? zSize : -zSize );

		newPos.y = startPos.y;

		transform.position = newPos;
	}

	public void Lock()
	{
		if(lockedIn)
			return;

		lockedIn = true;

		var newP = transform.position;
		newP.y -= lockDownDist;

		transform.position = newP;

		lineR.material = laserLocked;
	}

	public void UnLock()
	{
		if(!lockedIn)
			return;

		lockedIn = false;

		var newP = transform.position;
		newP.y += lockDownDist;

		transform.position = newP;

		lineR.material = laserUnlocked;
	}
}