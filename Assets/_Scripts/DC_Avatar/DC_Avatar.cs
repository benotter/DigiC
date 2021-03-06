﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_Avatar : NetworkBehaviour 
{
    // Client-Side Variables
    public DC_LocalPlayer localPlayer;

    [Space(10)]

    public int rightStartToolIndex = -1;
    public int leftStartToolIndex = -1;

    public GameObject[] toolList = new GameObject[0];
    
    [Space(10)]

    public Camera avatarCam;

    [Space(10)]

    public DC_Avatar_Muzzle muzzle;
    public DC_Avatar_Muzzle_Chord muzzleChord;

    public DC_Avatar_Chest chest;
    public DC_Avatar_Chest_Chord chestChord;

    public DC_Avatar_Paw rightPaw;
    public DC_Avatar_Paw leftPaw;

    [Space(10)]

    public DC_Avatar_Tool rightTool;
    public DC_Avatar_Tool leftTool;

    [Space(10)]

    [HideInInspector] public bool inMove = false;

    [HideInInspector] public bool controllersFlipped = false;

    // Server-Side Variables

    [SyncVar] public float gravity = 1f;
    [SyncVar] public float drag = 1f;
    [SyncVar] public float terminalVelocity = 10f;

    [Space(10)]

    [SyncVar] public GameObject playerO;

    [Space(10)]

    [SyncVar] public GameObject rightToolO;
    [SyncVar] public GameObject leftToolO;
    
    [Space(10)]

    [SyncVar] public bool linked = false;
    

    // Private Client-Side Variables

    private CharacterController coll;

    private bool wasLinked = false;
    
    private Vector3 targetMove = Vector3.zero;
    private Vector3 lastHmdPos = Vector3.zero;
    private Vector3 lastPos = Vector3.zero;
    public Vector3 moveVelocity = Vector3.zero;
    public Vector3 currentVelocity = Vector3.zero;

    private CollisionFlags lastColFlags;

    private bool DestroyNextUpdate = false;


    void Start()
    {
        NetworkTransform netT = GetComponent<NetworkTransform>();
        foreach(NetworkTransformChild childT in GetComponents<NetworkTransformChild>())
        {
            childT.sendInterval = netT.sendInterval;
            childT.interpolateMovement = netT.interpolateMovement;
            childT.interpolateRotation = netT.interpolateRotation;
            childT.rotationSyncCompression = netT.rotationSyncCompression;
        }
    }

    public override void OnStartServer()
    {
        coll = GetComponent<CharacterController>();
    }

    public override void OnStartClient()
    {

    }

    public override void OnStartAuthority()
    {
        avatarCam.enabled = true;
        coll = GetComponent<CharacterController>();

        if(rightStartToolIndex > -1)
            CmdRequestTool(rightStartToolIndex, 0);

        if(leftStartToolIndex > -1)
            CmdRequestTool(leftStartToolIndex, 1);

        SetupClientLayers();
    }

    public override void OnStopAuthority()
    {
        avatarCam.enabled = false;
    }

    [Client] public void SetupClientLayers()
    {
        var mD = muzzle.transform.GetChild(0);
        var mcD = muzzleChord.transform.GetChild(0);

        var cD = chest.transform.GetChild(0);
        var ccD = chestChord.transform.GetChild(0);
        
        int avaLocalLayer = LayerMask.NameToLayer("Avatar Local");

        if(mD)
            mD.gameObject.layer = avaLocalLayer;

        if(mcD)
            mcD.gameObject.layer = avaLocalLayer;

        if(cD)
            cD.gameObject.layer = avaLocalLayer;

        if(ccD)
            ccD.gameObject.layer = avaLocalLayer;
    }

    [Server] public void BustAvatar() 
    {
        Debug.Log("Avatar Busted!");
        playerO.GetComponent<DC_Player>().ClearAvatar();
    }

    [Server] public void BoltStrike(DC_Avatar_Part bodyP, DC_Bolt bolt)
    {
        BodyParts part = bodyP.GetBodyPart();

        switch(part)
        {
            case BodyParts.Chest:
            break;

            case BodyParts.Muzzle:
            break;

            case BodyParts.RightPaw:
            break;

            case BodyParts.LeftPaw:
            break;
            

            case BodyParts.ChestChord:
                BustAvatar();
            break;

            case BodyParts.MuzzleChord:
                BustAvatar();
            break;
        }

        Debug.Log("Body Part Hit!: " +bodyP.gameObject.name + " : " + bodyP.health);

        if(part != BodyParts.ChestChord && part != BodyParts.MuzzleChord)
            RpcSetPartHealth(part, bodyP.health);
    }

    [Client] public void SyncAvatarTools(DC_AvatarSync_Handle rightHandle, DC_AvatarSync_Handle leftHandle)
    {
        if(rightTool)
            rightTool.UpdateState(rightHandle);

        if(leftTool)
            leftTool.UpdateState(leftHandle);
    }

    [Client] public void StartMove()
    {
        inMove = true;
    }

    [Client] public void StopMove()
    {
        inMove = false;
    }

    [Client] public void MoveTo(Vector3 worldPoint, Vector3 moveTarget)
    {
        MoveTo(worldPoint);
    }

    [Client] public void MoveTo(Vector3 worldPoint)
    {
        targetMove += worldPoint;
    }

    [Client] public void ResetVelocity()
    {
        currentVelocity = Vector3.zero;
    }

    [Client] public void SetLocalPlayer(DC_LocalPlayer p = null)
    {
        localPlayer = p;
    }


    // Server-Side Commands    
    [Command] public void CmdStartLink()
    {
        linked = true;
        wasLinked = true;
    }
    
    [Command] public void CmdStopLink()
    {
        linked = false;
    }

    [Command] public void CmdRequestTool(int toolIndex, int hand)
    {
        if(toolIndex >= toolList.Length)
            return;

        GameObject toolB = toolList[toolIndex];

        GameObject toolO = Instantiate(toolB);
        
        if(NetworkServer.SpawnWithClientAuthority(toolO, playerO))
        {
            DC_Avatar_Tool tool = toolO.GetComponent<DC_Avatar_Tool>();

            tool.avatarO = gameObject;
            tool.playerO = playerO;

            if(hand == 0)
            {
                if(rightToolO)
                    Network.Destroy(rightToolO);

                rightToolO = toolO;
            }
            else
            {
                if(leftToolO)
                    Network.Destroy(leftToolO);

                leftToolO = toolO;
            }

            tool.RpcSetAvatar(gameObject);
            RpcSetAvatarTool(toolO, hand);
        }
    }


    // Client-Side Commands
    [ClientRpc] public void RpcSetPartHealth(BodyParts part, int health) 
    {
        DC_Avatar_Part p = chest;

        switch(part)
        {
            case BodyParts.Chest:
                p = chest;
            break;

            case BodyParts.Muzzle:
                p = muzzle;
            break;

            case BodyParts.RightPaw:
                p = rightPaw;
            break;

            case BodyParts.LeftPaw:
                p = leftPaw;
            break;
        }

        p.SetHealth(health);
        p.OnServerUpdate();
    }

    [ClientRpc] public void RpcSetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    [ClientRpc] public void RpcSetAvatarTool(GameObject toolO, int hand)
    {
        DC_Avatar_Tool tool = toolO.GetComponent<DC_Avatar_Tool>();
        if(tool)
        {
            Transform h = hand == 0 ? rightPaw.transform : leftPaw.transform;

            toolO.transform.parent = h;
            toolO.transform.localEulerAngles = Vector3.zero;
            toolO.transform.localPosition = Vector3.zero;

            if(hand == 0)
                rightTool = tool;
            else
                leftTool = tool;
        }
    }

    [ClientRpc] public void RpcClearTool(int hand)
    {
        if(hand == 0)
            rightToolO = null;
        else
            leftToolO = null;
    }

    [ClientRpc] public void RpcAvatarBust() 
    {
        DestroyNextUpdate = true;
    }
}

public partial class DC_Avatar 
{
    public enum BodyParts 
    {
        None,
        Muzzle,
        Chest,
        RightPaw,
        LeftPaw,
        MuzzleChord,
        ChestChord
    }
}