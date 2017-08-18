using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_Avatar : NetworkBehaviour 
{
    // Client-Side Variables
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

    public int moveBeamLocalRes = 15;
    public int moveBeamRemoteRes = 5;

    [Space(10)]

    public float beamJut = 0.2f;


    [Space(10)]

    // Server-Side Variables

    [SyncVar]
    public GameObject playerO;

    [Space(10)]


    [SyncVar]
    public float gravity = 1f;

    [SyncVar]
    public float drag = 1f;

    [SyncVar]
    public float terminalVelocity = 10f;


    [Space(10)]


    [SyncVar]
    public bool linked = false;

    [SyncVar]
    public bool inMove = false;


    // Public Client-Side Variables

    [HideInInspector]
    public bool controllersFlipped = false;
    
    // Private Client-Side Variables

    private bool wasLinked = false;
    private CharacterController coll;

    private Vector3 targetMove = Vector3.zero;
    
    private Vector3 lastHmdPos = Vector3.zero;

    private Vector3 lastPos = Vector3.zero;

    public Vector3 moveVelocity = Vector3.zero;

    public Vector3 currentVelocity = Vector3.zero;

    private CollisionFlags lastColFlags;


    private LineRenderer lineR;
    private DC_Avatar_MoveBeam moveB;

    // Private Server-Side Variables

    [SyncVar]
    private Vector3 moveBeamPoint = Vector3.zero;

    [SyncVar]
    private bool moveBeamRightH = true;

    public override void OnStartServer()
    {
        // coll = GetComponent<CharacterController>();
    }

    public override void OnStartClient()
    {
        lineR = GetComponent<LineRenderer>();
        moveB = new DC_Avatar_MoveBeam(moveBeamRemoteRes);
    }

    public override void OnStartAuthority()
    {
        moveB = new DC_Avatar_MoveBeam(moveBeamLocalRes);

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

    public void MoveTo(Vector3 worldPoint)
    {
        targetMove += worldPoint;
    }

    public void ResetVelocity()
    {
        currentVelocity = Vector3.zero;
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
        }

        UpdateMoveBeam();

        if(hasAuthority)
            UpdatePosition();
    }

    public void UpdateMoveBeam()
    {
        if(inMove)
        {
            if(!lineR.enabled)
                lineR.enabled = true;

            var start = moveBeamRightH && !controllersFlipped ? 
                rightPaw.transform : leftPaw.transform;

            moveB.UpdateBeam(start.position, start.position + (start.forward * beamJut), moveVelocity, moveBeamPoint);
            lineR.positionCount = moveB.resolution;
            lineR.SetPositions(moveB.UpdateLinePoints());
        }
        else
        {
            if(lineR.enabled)
                lineR.enabled = false;
        }
    }
    public void UpdatePosition()
    {
        if(!inMove && (lastColFlags & CollisionFlags.Below) == 0)
            currentVelocity.y -= gravity * Time.deltaTime;

        lastColFlags = coll.Move(targetMove + currentVelocity * Time.deltaTime);

        currentVelocity -= (drag * currentVelocity) * Time.deltaTime;

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

    public void SetLocalPlayer(DC_LocalPlayer p = null)
    {
        localPlayer = p;
    }

    public void SetAvatarTool(DC_Avatar_Tool_Base tool = null, int hand = 0)
    {
        GameObject handO;
        if(hand == 0)
        {
            if(rightTool)
                rightTool.gameObject.SetActive(false);
                
            rightTool = tool;
            handO = rightPaw;
        }
        else
        {
            if(leftTool)
                leftTool.gameObject.SetActive(false);

            leftTool = tool;
            handO = leftPaw;
        }
        
        if(tool && tool.avatar != this)
        {
            tool.avatar = this;
            tool.paw = handO;

            tool.gameObject.SetActive(true);
        }
    }

    public void SyncAvatarTools(DC_AvatarSync_Handle rightHandle, DC_AvatarSync_Handle leftHandle)
    {
        if(rightTool)
            rightTool.UpdateState(rightHandle);

        if(leftTool)
            leftTool.UpdateState(leftHandle);
    }

    // Server-Side Commands

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

    [Command]
    public void CmdSetMoveBeamPoint(Vector3 point)
    {
        moveBeamPoint = point;
    }

    [Command]
    public void CmdSetMoveBeamHand(bool right)
    {
        moveBeamRightH = right;
    }

    // Client-Side Commands

    [ClientRpc]
    public void RpcSetPosition(Vector3 pos)
    {
        transform.position = pos;
    }
}

[System.Serializable]
public class DC_Avatar_MoveBeam
{
    public int resolution;

    public Vector3[] linePoints;
    public Vector3[] pN = new Vector3[] 
    {
        Vector3.zero,
        Vector3.zero,
        Vector3.zero,
        Vector3.zero,
        Vector3.zero,
        Vector3.zero,
        Vector3.zero,
    };

    public DC_Avatar_MoveBeam(int resolution = 2)
    {
        this.resolution = resolution;
        linePoints = new Vector3[resolution];
    }

    // The Double-ups ensures sharp straight lines with the Bezier curves
    public void UpdateBeam(Vector3 startPos, Vector3 jut, Vector3 vel, Vector3 endPos)
    {
        pN[0] = startPos;
        pN[1] = startPos;

        pN[2] = jut;
        pN[3] = jut;

        pN[4] = startPos + vel;

        pN[5] = endPos;
        pN[6] = endPos;

    }
    public Vector3[] UpdateLinePoints()
    {
        for(int i = 0; i < resolution; i++)
            linePoints[i] = BezierNOpt(pN, ( (float) i / (float) resolution ) );

        return linePoints;
    }

    public void UpdateResolution(int res)
    {
        resolution = res;
        linePoints = new Vector3[res];
    }

    Vector3 BezierNOpt(Vector3 [] pN, float prog, Vector3 [] wrkCpy = null, int indOff = -1)
	{
		if(wrkCpy == null)
		{
			indOff = pN.Length;
			wrkCpy = new Vector3[pN.Length];
			pN.CopyTo(wrkCpy, 0);	
		}

		if(indOff == 1)
			return wrkCpy[0];

		for(int i = 0; i < indOff - 1; i++)
			wrkCpy[i] = Vector3.Lerp(wrkCpy[i], wrkCpy[i + 1], prog);
		
		return BezierNOpt(null, prog, wrkCpy, indOff - 1);
	}
}