﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class DC_AvatarSync : MonoBehaviour 
{
	public DC_LocalPlayer localPlayer;
	public GameObject hmdTriggerObj;

	[Space(10)]
	public DC_Avatar avatar;

	[Space(10)]

	public float relinkDistance = 0.3f;

	[Space(10)]
	public GameObject model;

	[Space(10)]
	public GameObject screen;
	public GameObject screenCover;

	[Space(10)]

	public DC_AvatarSync_Handle rightHandle;
	public DC_AvatarSync_Handle leftHandle;


	void Start () 
	{
		originalPar = transform.parent;
		originalPos = transform.position;
		originalRot = transform.rotation;
	}
	
	void Update () 
	{
		LinkUpdate();
		if(linkReset && Vector3.Distance(localPlayer.HMD.transform.position, transform.position) >= relinkDistance)
			linkReset = false;
	}
}

// Avatar Link Stuff
public partial class DC_AvatarSync 
{
	public bool canLink = false;
	public bool linked = false;
	public bool linkReset = false;


	private Transform originalPar;
	private Vector3 originalPos;
	private Quaternion originalRot;


	public void LinkUpdate()
	{
		canLink = (avatar && (rightHandle.inUse && leftHandle.inUse) && !linkReset);
	}


	public void SetAvatar(DC_Avatar a = null)
	{
		avatar = a;
		screenCover.SetActive(a == null);
		
		if(a)
			a.SetLocalPlayer(localPlayer);
	}

	public void StartLink()
	{
		if(!linked && canLink)
		{
			linked = true;
			
			avatar.StartLink();
			
			SwapPlayerAvatarCams();
			AttackToHMD();
		}
	}

	public void StopLink()
	{
		if(linked)
		{
			linked = false;
			linkReset = true;

			if(avatar)
				avatar.StopLink();
			
			SwapPlayerAvatarCams();
			DetachFromHMD();
		}
	}

	public void SwapPlayerAvatarCams()
	{
		if(!avatar)
		{
			localPlayer.HMD.GetComponent<Camera>().depth = 0;
			return;
		}

		var playerCam = localPlayer.HMD.GetComponent<Camera>();
		var avatarCam = avatar.avatarCam;

		float depth = playerCam.depth;

		playerCam.depth = avatarCam.depth;
		avatarCam.depth = depth;
	}

	public void AttackToHMD()
	{
		transform.parent = localPlayer.HMD.transform;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.Euler(Vector3.zero);
	}

	public void DetachFromHMD()
	{
		transform.parent = originalPar;
	}
}

public partial class DC_AvatarSync 
{
	void OnTriggerEnter(Collider col)
	{
		if(col.gameObject == hmdTriggerObj)
			StartLink();
	}
}