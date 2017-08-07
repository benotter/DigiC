using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_Avatar 
{
    public Camera avatarCam;

    [Space(10)]

    public GameObject muzzle;
    public GameObject chest;
    public GameObject rightPaw;
    public GameObject leftPaw;

    [Space(10)]

    public DC_LocalPlayer localPlayer;

    public override void OnStartClient() 
    {
        if(!isLocalPlayer)
        {
            if(avatarCam)
                Destroy(avatarCam);
        }
    }

    void ClientUpdate()
    {
        if(isLocalPlayer)
        {
            if(linked)
                UpdateBody();
        }
    }

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
