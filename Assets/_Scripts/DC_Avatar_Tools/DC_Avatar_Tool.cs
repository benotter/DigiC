using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DC_Avatar_Tool : NetworkBehaviour
{
	// Client-Side Variables

	public float triggerMinPull = 0.15f;
	public float triggerMinClick = 1f;


	[Space(10)]
	[HideInInspector] public DC_Player player;
	[HideInInspector] public DC_Avatar avatar;

	[Space(10)]

	[HideInInspector] public bool cleared = true;

	[HideInInspector] public float trigger = 0f;
	[HideInInspector] public float touchX = 0f;
	[HideInInspector] public float touchY = 0f;
	[HideInInspector] public bool touch = false;
	[HideInInspector] public bool touchClick = false;
	[HideInInspector] public bool gripButton = false;
	

	// Server-Side Variables

	[HideInInspector] [SyncVar] public GameObject playerO;
	[HideInInspector] [SyncVar] public GameObject avatarO;
	[HideInInspector] [SyncVar] public PlayerTool.Hand hand = PlayerTool.Hand.None;
	
	// Private / Protected Client-Side Variables

	// Private Server-Side Variables

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

	// Server-Side Commands

	// Client-Side Commands

	[ClientRpc] public void RpcSetAvatar(GameObject aO)
	{
		DC_Avatar avatar = aO.GetComponent<DC_Avatar>();
		DC_Player player = avatar.playerO.GetComponent<DC_Player>();	

		this.avatar = avatar;
		this.player = player;
	}
}

