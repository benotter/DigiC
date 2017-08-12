using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_Avatar_Tool_Base : MonoBehaviour 
{	
	public DC_Avatar avatar;
	public GameObject hand;

	public float trigger = 0f;

	public float touchX = 0f;
	public float touchY = 0f;

	public bool touch = false;
	public bool touchClick = false;

	public bool gripButton = false;

	public bool cleared = true;

	public void UpdateState(DC_AvatarSync_Handle handle)
	{
		trigger = handle.trigger;

		touchX = handle.touchX;
		touchY = handle.touchY;

		touch = handle.touch;
		touchClick = handle.touchClick;

		gripButton = handle.gripButton;

		cleared = false;
	}

	public void ClearState()
	{
		trigger = 0f;

		touchX = 0f;
		touchY = 0f;

		touch = false;
		touchClick = false;
		
		gripButton = false;

		cleared = true;
	}
}
