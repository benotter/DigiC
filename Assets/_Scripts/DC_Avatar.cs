﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_Avatar : NetworkBehaviour 
{
    public DC_LocalPlayer localPlayer;

    [Space(10)]

    public Camera avatarCam;

    [Space(10)]

    public GameObject muzzle;
    public GameObject chest;
    public GameObject rightPaw;
    public GameObject leftPaw;

    [Space(10)]

    [SyncVar]
    public GameObject player;

    [Space(10)]

    [SyncVar]
    public bool linked = false;

    [SyncVar]
    public bool inMove = false;

    private CapsuleCollider coll;

    
    public void SetLocalPlayer(DC_LocalPlayer p = null)
    {
        localPlayer = p;
    }

    public void StartLink()
    {
        linked = true;
    }

    public void StopLink()
    {
        linked = false;
    }

    [Command]
    public void CmdStartMove()
    {
        inMove = true;
    }

    [Command]
    public void CmdStopMove()
    {
        inMove = false;
    }

    void Start()
    {
        
    }
    
    public override void OnStartServer()
    {
        coll = GetComponent<CapsuleCollider>();
    }

    public override void OnStartClient()
    {
        
    }

    public override void OnStartAuthority()
    {
        avatarCam.enabled = true;

        var layer = LayerMask.NameToLayer("Avatar_Local");

        muzzle.transform.GetChild(0).gameObject.layer = layer;
        chest.transform.GetChild(0).gameObject.layer = layer;

        coll = GetComponent<CapsuleCollider>();
    }
    public override void OnStopAuthority()
    {
        avatarCam.enabled = false;
    }

	void Update()
    {
        if(isClient)
            ClientUpdate();
        else if(isServer)
            ServerUpdate();
    }

    void ServerUpdate()
    {
        UpdateCollider();
    }

    void ClientUpdate()
    {
        if(hasAuthority)
        {
            if(linked)
                UpdateBody();

            UpdateCollider();
        }
    }

    void UpdateCollider()
    {
        var mP = muzzle.transform.localPosition;
        coll.height = mP.y - (coll.radius * 2f);
        coll.center = new Vector3(mP.x, mP.y / 2f, mP.z);
    }

    public void UpdateBody()
    {
        var mT = muzzle.transform;
        var cT = chest.transform;
        var rpT = rightPaw.transform;
        var lpT = leftPaw.transform;

        mT.localPosition = localPlayer.hmdPos;
        mT.localRotation = localPlayer.hmdRot;

        cT.localPosition = mT.localPosition;

        rpT.localPosition = localPlayer.rightPos;
        rpT.localRotation = localPlayer.rightRot;

        lpT.localPosition = localPlayer.leftPos;
        lpT.localRotation = localPlayer.leftRot;
    }
}