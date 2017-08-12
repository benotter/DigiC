using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_Avatar : NetworkBehaviour 
{
    public DC_LocalPlayer localPlayer;

    public DC_Avatar_Tool_Base rightTool;
    public DC_Avatar_Tool_Base leftTool;


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
    public float gravity = 1f;

    [SyncVar]
    public float terminalVelocity = 10f;

    [Space(10)]

    [SyncVar]
    public bool linked = false;

    [SyncVar]
    public bool inMove = false;


    private bool wasLinked = false;
    private CharacterController coll;

    private Vector3 targetMove = Vector3.zero;
    
    private Vector3 lastHmdPos = Vector3.zero;
    

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

    [Command]
    public void CmdStartLink()
    {
        linked = true;
        wasLinked = true;
    }

    [Command]
    public void CmdStopLink()
    {
        linked = false;
    }

    public void SetLocalPlayer(DC_LocalPlayer p = null)
    {
        localPlayer = p;
    }

    public void SetAvatarTool(DC_Avatar_Tool_Base tool = null, int hand = 0)
    {
        GameObject handO;
        if(hand == 0)
        {
            rightTool = tool;
            handO = rightPaw;
        }
        else
        {
            leftTool = tool;
            handO = leftPaw;
        }
        
        if(tool && tool.avatar != this)
        {
            tool.avatar = this;
            tool.hand = handO;
        }
            
    }

    public void SyncAvatarTools(DC_AvatarSync_Handle rightHandle, DC_AvatarSync_Handle leftHandle)
    {
        if(rightTool)
            rightTool.UpdateState(rightHandle);

        if(leftTool)
            leftTool.UpdateState(leftHandle);
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

        var layer = LayerMask.NameToLayer("Avatar_Local");

        muzzle.transform.GetChild(0).gameObject.layer = layer;
        chest.transform.GetChild(0).gameObject.layer = layer;

        coll = GetComponent<CharacterController>();

        if(rightTool)
            SetAvatarTool(rightTool, 0);
        
        if(leftTool)
            SetAvatarTool(leftTool, 1);
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

            UpdatePosition();
        }
    }

    public void UpdateBody()
    {
        var mT = muzzle.transform;
        var cT = chest.transform;

        var rpT = rightPaw.transform;
        var lpT = leftPaw.transform;

        var hmdP = localPlayer.hmdPos;
           
        mT.localPosition = new Vector3(0f, hmdP.y, 0f);
        mT.localRotation = localPlayer.hmdRot;

        cT.localPosition = mT.localPosition;

        var rP = localPlayer.rightPos;

        rpT.localPosition = new Vector3(rP.x - hmdP.x, rP.y, rP.z - hmdP.z);
        rpT.localRotation = localPlayer.rightRot;

        var lP = localPlayer.leftPos;

        lpT.localPosition = new Vector3(lP.x - hmdP.x, lP.y, lP.z - hmdP.z);
        lpT.localRotation = localPlayer.leftRot;

        var acP = avatarCam.transform.parent;
        var aP = avatarCam.transform.localPosition;

        var newAP = -aP;
        newAP.y = 0;

        acP.localPosition = newAP;
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

            Debug.Log("Distance: " + Vector3.Distance(hmdP, lastHmdPos));
            lastHmdPos = hmdP;

            MoveTo(move);
        }    
    }

    public void MoveTo(Vector3 worldPoint)
    {
        targetMove += worldPoint;
    }

    public void UpdatePosition()
    {
        if(!inMove)
        {
            // SIDE EFFECTS IN ASSIGNMENT? WHY DO YOU HATE THE COMPUTATION GODS?
            float gSpeed = -(targetMove.y += -(gravity * Time.deltaTime));

            Debug.Log("GSpeed: " + gSpeed);

            if(gSpeed >= terminalVelocity)
                targetMove.y = -terminalVelocity;
        }

        Debug.Log("TargetMove: " + targetMove);
        var colF = coll.Move(targetMove);

        targetMove = Vector3.zero;
    }
}
