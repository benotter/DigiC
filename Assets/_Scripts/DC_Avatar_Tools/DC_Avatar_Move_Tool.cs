using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(LineRenderer))]
public class DC_Avatar_Move_Tool : DC_Avatar_Tool
{
    // Client-Side Variables
    public LayerMask hitLayers;

    [Space(10)]
    
    public float maxMoveSpeed = 10f;
    public float maxMoveDistance = 1000f;
    public float cushionSize = 10f;
    public float deadZone = 0.5f;

    [Space(10)]
    
    public float retractRate = 1f;
    public float maxRetractRate = 20f;

    [Space(10)]

    public int moveBeamLocalRes = 15;
    public int moveBeamRemoteRes = 5;

    [Space(10)]

    public float beamJut = 0.2f;

    [Range(0f, 1f)] public float velPoint = 0.75f;

    [Space(10)]

    public Color canMoveColor = Color.green;
    public Color cannotMoveColor = Color.red;

    [Space(10)]

    // Server-Side Variables

    [SyncVar] public bool moving = false;

    [SyncVar] public Vector3 movePoint = Vector3.zero;


    // Private Client-Side Variables
    private bool canMove = false;
    private bool seeking = false;
    private bool retracting = false;
    private bool moveBlock = false;

    private float movePointDistance = 0f;
    private float currentRetract = 0f;

    private Vector3 localMovePoint = Vector3.zero;
    private Vector3 targetPoint = Vector3.zero;
    private Vector3 spacePoint;

    private GameObject laser;
    private MeshRenderer laserRend;
    private LineRenderer lineR;
    private DC_Avatar_MoveBeam moveB;

    // Private Server-Side Variables


    void Start()
    {
        lineR = GetComponent<LineRenderer>();
    }

    public override void OnStartClient()
    {
        moveB = new DC_Avatar_MoveBeam(moveBeamRemoteRes);
    }

    public override void OnStartAuthority()
    {
        moveB = new DC_Avatar_MoveBeam(moveBeamLocalRes);
    }

    void Update() 
    {
        if(isClient)
            ClientUpdate();
        else if(isServer)
            ServerUpdate();

    }

    public void ServerUpdate()
    {

    }

    public void ClientUpdate()
    {
        if(hasAuthority)
            UpdateMovement();

        if(moving)
            UpdateMoveBeam();
        else if(lineR.enabled)
            lineR.enabled = false;
    }

    public void UpdateMoveBeam()
    {
        if(!lineR.enabled)
            lineR.enabled = true;

        var start = transform;

        var st = start.position;
        var jut = start.position + (start.forward * beamJut);

        var spaceP = spacePoint;

        if(spaceP == Vector3.zero)
            spaceP = transform.position;

        var vel = Vector3.Lerp(spaceP, movePoint, velPoint);

        var end = movePoint;

        moveB.UpdateBeam(st, jut, vel, end);
        lineR.positionCount = moveB.resolution;
        lineR.SetPositions(moveB.UpdateLinePoints());
    }

    void UpdateMovement()
    {
        if(trigger > 0.15f)
        {
            if(!moving)
                UpdateMovePoint();

            if(!moving && !seeking)
                seeking = true;

            if(trigger == 1f && !moving && !moveBlock) 
            {
                if(canMove)
                {
                    CmdSetMovePoint(localMovePoint);
                    CmdStartMove();
                }
                    
                else
                    moveBlock = true;
            }

            if(moveBlock && trigger < 0.97f)
                moveBlock = false;

            if(touch && touchClick && touchY < 0f)
            {
                if(currentRetract < maxRetractRate)
                    currentRetract += retractRate;
            }
            else if(currentRetract > 0)
            {
                currentRetract -= retractRate;

                if(currentRetract < 0)
                    currentRetract = 0;
            }
        }
        else
        {
            if(seeking)
                seeking = false;

            if(moving)
                CmdStopMove();
        }

        if(seeking && !moving)
        {
            UpdateMovePoint();
            UpdateLaser();
        }

        if(!seeking || moving)
            if(laser && laser.activeSelf)
                laser.SetActive(false);

        if(moving)
        {
            UpdateTargetPoint();

            if(currentRetract > 0)
            {
                movePointDistance -= currentRetract * Time.deltaTime;
                if(movePointDistance < 0)
                {
                    movePointDistance = 0f;
                    moveBlock = true;

                    avatar.StopMove();
                    avatar.ResetVelocity();
                }
            }

            if(!avatar.inMove)
                avatar.StartMove();

            avatar.MoveTo(targetPoint, spacePoint);
        }
        else
        {
            if(avatar.inMove)
                avatar.StopMove();
        }
    }

    void UpdateMovePoint()
    {
        var ht = transform;

        Ray ray = new Ray(ht.position, ht.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, maxMoveDistance, hitLayers))
        {
            localMovePoint = hit.point;
            movePointDistance = hit.distance;
            canMove = true;
        }
        else
        {
            if(localMovePoint != Vector3.zero)
                localMovePoint = Vector3.zero;

            if(movePointDistance != 0)
                movePointDistance = 0f;

            canMove = false;
        }
    }  

    void UpdateTargetPoint()
    {
        var ht = transform;
        var at = avatar.transform;

        spacePoint = movePoint - (ht.forward * movePointDistance);
        spacePoint -= ht.localPosition;

        var dist = Vector3.Distance(spacePoint, at.position);

        var h = spacePoint - at.position;
        var d = h.magnitude;
        var dir = h / d;

        if(dist > cushionSize)
            dist = cushionSize;
        
        targetPoint = dir * ( (dist / cushionSize) * maxMoveSpeed);
    }

    void UpdateLaser()
    {
        if(!laser)
            CreateLaser();

        if(!laser.activeSelf)
            laser.SetActive(true);

        float laserLength = movePointDistance;

        if(!canMove)
            laserLength = maxMoveDistance;

        var laserS = laser.transform.localScale;
        laserS.z = laserLength;
        laser.transform.localScale = laserS;

        var laserP = laser.transform.localPosition;
        laserP.z = laserLength / 2f;
        laser.transform.localPosition = laserP;

        Color laserC = canMove ? canMoveColor : cannotMoveColor;

        if(laserRend.material.color != laserC)
            laserRend.material.color = laserC;
    } 

    void CreateLaser()
    {
        var l = GameObject.CreatePrimitive(PrimitiveType.Cube);

        var rb = l.GetComponent<Rigidbody>();
        if(rb)
            Destroy(rb);

        var col = l.GetComponent<Collider>();

        if(col)
            Destroy(col);

        var mr = l.GetComponent<MeshRenderer>();

        l.transform.localScale = new Vector3(0.05f, 0.05f, 0f);

        l.transform.parent = transform;

        l.transform.localPosition = Vector3.zero;
        l.transform.localEulerAngles = Vector3.zero;

        l.name = "Move Laser";

        laser = l;
        laserRend = mr;

        l.SetActive(false);
    }

    // Server-Side Commands

    [Command] public void CmdSetMovePoint(Vector3 pos)
    {
        movePoint = pos;
    }

    [Command] public void CmdStartMove()
    {
        moving = true;
    }

    [Command] public void CmdStopMove()
    {
        moving = false;
    }

    // Client-Side Commands
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

        pN[4] = vel;

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