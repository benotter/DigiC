using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_Avatar_Tool_Base : MonoBehaviour
{	
	public int toolIndex = 0;
	
	public DC_Avatar avatar;
	public GameObject paw;

	[Space(10)]

	public PlayerTool.Hand hand;

	[Space(10)]

	public bool blockChange = false;
	

	[HideInInspector]
	public float trigger = 0f;

	[HideInInspector]
	public float touchX = 0f;
	[HideInInspector]
	public float touchY = 0f;

	[HideInInspector]
	public bool touch = false;
	[HideInInspector]
	public bool touchClick = false;

	[HideInInspector]
	public bool gripButton = false;
	
	[HideInInspector]
	public bool cleared = true;

	[HideInInspector]
	public bool hasAuthority = false;

	public void UpdateState(DC_AvatarSync_Handle handle)
	{
		hand = handle.hand;
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
		hand = PlayerTool.Hand.None;
		trigger = 0f;

		touchX = 0f;
		touchY = 0f;

		touch = false;
		touchClick = false;
		
		gripButton = false;

		cleared = true;
	}

	public virtual void ClientStart() {}

	public virtual void AuthorityStart() {}
	public virtual void ServerStart() {}

	public virtual void ToolUpdate() {}
	public virtual void ClientUpdate() {}
	public virtual void ServerUpdate() {}	
}
