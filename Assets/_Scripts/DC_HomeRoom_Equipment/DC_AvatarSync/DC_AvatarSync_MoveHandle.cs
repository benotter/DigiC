using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_AvatarSync_MoveHandle : SubToolBase 
{
	public DC_AvatarSync avatarSync;
	public GameObject holder;

	[Space(10)]
	public bool wasUsed = false;

	private Transform oldAST;

	void Start()
	{
		holder.transform.position = avatarSync.transform.position;
	}

	void Update () 
	{
		base.UpdateSubTool();

		if(inUse)
		{
			var newPos = avatarSync.transform.position;
			newPos.y = holder.transform.position.y;
			avatarSync.transform.position = newPos;

			var newHPos = transform.position;
			newHPos.x = avatarSync.transform.position.x;
			newHPos.z = avatarSync.transform.position.z;
			transform.position = newHPos;

			transform.eulerAngles = Vector3.zero;
		}
	}
}