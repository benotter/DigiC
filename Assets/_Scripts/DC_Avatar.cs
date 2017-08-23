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

    public GameObject muzzle;
    public GameObject chest;
    public GameObject rightPaw;
    public GameObject leftPaw;

    [Space(10)]

    public DC_Avatar_Tool rightTool;
    public DC_Avatar_Tool leftTool;

    [Space(10)]

    [HideInInspector] public bool inMove = false;

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


    // Public Client-Side Variables

    [HideInInspector] public bool controllersFlipped = false;
    
    // Private Client-Side Variables

    private CharacterController coll;

    private bool wasLinked = false;
    
    private Vector3 targetMove = Vector3.zero;
    
    private Vector3 lastHmdPos = Vector3.zero;
    private Vector3 lastPos = Vector3.zero;

    public Vector3 moveVelocity = Vector3.zero;
    public Vector3 currentVelocity = Vector3.zero;

    private CollisionFlags lastColFlags;


    void Start()
    {
        
    }

    public override void OnStartServer()
    {
        // coll = GetComponent<CharacterController>();
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

        coll = GetComponent<CharacterController>();

        if(rightStartToolIndex > -1)
            CmdRequestTool(rightStartToolIndex, 0);

        if(leftStartToolIndex > -1)
            CmdRequestTool(leftStartToolIndex, 1);
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
        // UpdateCharacterController();

    }

    void ClientUpdate()
    {
        if(hasAuthority)
        {
            if(linked)
            {
                UpdateBody();
                UpdateCharacterController();

            } else if(wasLinked)
            {
                wasLinked = false;

                if(rightTool && !rightTool.cleared)
                    rightTool.ClearState();

                if(leftTool && !leftTool.cleared)
                    leftTool.ClearState();
            }

            if(inMove)
            {
                if(lastPos != transform.position)
                {
                    moveVelocity = (transform.position - lastPos) / Time.deltaTime;
                    lastPos = transform.position;
                }

                if(currentVelocity != Vector3.zero)
                    ResetVelocity();
            }
            else 
            {
                if(moveVelocity != Vector3.zero)
                {
                    currentVelocity += moveVelocity;
                    moveVelocity = Vector3.zero;
                }
            }

            UpdatePosition();
        }   
    }

    public void UpdatePosition()
    {
        if(!inMove && (lastColFlags & CollisionFlags.Below) == 0)
            currentVelocity.y -= gravity * Time.deltaTime;

        var move = targetMove + currentVelocity * Time.deltaTime;

        if(Mathf.Abs(move.x) > terminalVelocity)
            move.x = move.x > 0 ? terminalVelocity : -terminalVelocity;
                    
        if(Mathf.Abs(move.y) > terminalVelocity)
            move.y = move.y > 0 ? terminalVelocity : -terminalVelocity;

        if(Mathf.Abs(move.z) > terminalVelocity)
            move.z = move.z > 0 ? terminalVelocity : -terminalVelocity;

        lastColFlags = coll.Move(move);

        if(lastColFlags == CollisionFlags.None)
            currentVelocity -= (drag * currentVelocity) * Time.deltaTime;
        else
            ResetVelocity();

        targetMove = Vector3.zero;
    }

    public void UpdateBody()
    {
        var mT = muzzle.transform;
        var cT = chest.transform;

        var hmdP = localPlayer.hmdPos;
           
        mT.localPosition = new Vector3(0f, hmdP.y, 0f);
        mT.localRotation = localPlayer.hmdRot;

        cT.localPosition = mT.localPosition;
        
        bool right = true, 
             left = false;

        if(controllersFlipped)
        {
            right = !right;
            left = !left;
        }

        UpdatePaw(rightPaw.transform, right);
        UpdatePaw(leftPaw.transform, left);

        var acP = avatarCam.transform.parent;
        var aP = avatarCam.transform.localPosition;

        var newAP = -aP;
        newAP.y = 0;

        acP.localPosition = newAP;
    }

    public void UpdatePaw(Transform p, bool rightH = true)
    {
        var hmdP = localPlayer.hmdPos;

        Vector3 pos = rightH ? localPlayer.rightPos : localPlayer.leftPos;
        Quaternion rot = rightH ? localPlayer.rightRot : localPlayer.leftRot;

        p.localPosition = new Vector3(pos.x - hmdP.x, pos.y, pos.z - hmdP.z);
        p.localRotation = rot;
    }

    void UpdateCharacterController()
    {
        var hmdP = localPlayer.hmdPos;
        if(lastHmdPos != hmdP)
        {
            var mP = muzzle.transform.localPosition;

            coll.height = mP.y;
            coll.center = new Vector3(0f, mP.y / 2f, 0f);

            var move = hmdP - lastHmdPos;
            move.y = 0f;

            lastHmdPos = hmdP;

            MoveTo(move);
        }    
    }

    public void StartMove()
    {
        inMove = true;
    }

    public void StopMove()
    {
        inMove = false;
    }

    public void MoveTo(Vector3 worldPoint)
    {
        targetMove += worldPoint;
    }

    public void MoveTo(Vector3 worldPoint, Vector3 moveTarget)
    {
        MoveTo(worldPoint);
    }

    public void ResetVelocity()
    {
        currentVelocity = Vector3.zero;
    }

    public void SetLocalPlayer(DC_LocalPlayer p = null)
    {
        localPlayer = p;
    }

    public void SyncAvatarTools(DC_AvatarSync_Handle rightHandle, DC_AvatarSync_Handle leftHandle)
    {
        if(rightTool)
            rightTool.UpdateState(rightHandle);

        if(leftTool)
            leftTool.UpdateState(leftHandle);
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
}

