using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_Avatar_Move_Tool : DC_Avatar_Tool_Base 
{
    public float maxMoveSpeed = 10f;
    public float maxMoveDistance = 1000f;
    public float cushionSize = 10f;
    public float deadZone = 0.5f;
    
    public float retractRate = 1f;
    public float maxRetractRate = 20f;

    private bool seeking = false;
    private bool moving = false;

    private bool canMove = false;

    private Vector3 movePoint = Vector3.zero;
    private float movePointDistance = 0f;

    private Vector3 targetPoint = Vector3.zero;

    private bool retracting = false;

    private GameObject laser;
    private MeshRenderer laserRend;

    void Start()
    {
        
    }

    void Update()
    {
        if(trigger > 0.15f)
        {
            if(!moving)
                UpdateMovePoint();

            if(canMove && !seeking)
                seeking = true;

            if(canMove && trigger == 1f && !moving)
                moving = true;

            

            if(touch && touchClick && touchY < 0f)
                movePointDistance -= retractRate * Time.deltaTime;

            Debug.Log("TouchY: " + touchY);
            Debug.Log("MoveDist: " + movePointDistance);
        }
        else
        {
            if(seeking)
                seeking = false;

            if(moving)
                moving = false;
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

            if(!avatar.inMove)
            {
                avatar.CmdStartMove();
                avatar.CmdSetMoveBeamPoint(movePoint);
                avatar.CmdSetMoveBeamHand(hand == PlayerTool.Hand.Right);
            }

            avatar.MoveTo(targetPoint);
        }
        else
        {
            if(avatar.inMove)
                avatar.CmdStopMove();
        }
    }

    void UpdateMovePoint()
    {
        var ht = paw.transform;

        Ray ray = new Ray(ht.position, ht.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, maxMoveDistance))
        {
            movePoint = hit.point;
            movePointDistance = hit.distance;
            canMove = true;
        }
        else
        {
            if(movePoint != Vector3.zero)
                movePoint = Vector3.zero;

            if(movePointDistance != 0)
                movePointDistance = 0f;

            canMove = false;
        }
    }

    void UpdateLaser()
    {
        if(!laser)
            CreateLaser();

        if(!laser.activeSelf)
            laser.SetActive(true);

        var laserS = laser.transform.localScale;
        laserS.z = movePointDistance;
        laser.transform.localScale = laserS;

        var laserP = laser.transform.localPosition;
        laserP.z = movePointDistance / 2f;
        laser.transform.localPosition = laserP;
    }

    void UpdateTargetPoint()
    {
        var ht = paw.transform;
        var at = avatar.transform;

        var spacePoint = movePoint - (ht.forward * movePointDistance);
        spacePoint -= ht.localPosition;

        var dist = Vector3.Distance(spacePoint, at.position);

        var h = spacePoint - at.position;
        var d = h.magnitude;
        var dir = h / d;

        if(dist > cushionSize)
            dist = cushionSize;
        
        targetPoint = dir * ( (dist / cushionSize) * maxMoveSpeed);
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

        l.transform.parent = paw.transform;

        l.transform.localPosition = Vector3.zero;
        l.transform.localEulerAngles = Vector3.zero;

        l.name = "Move Laser";

        laser = l;
        laserRend = mr;

        l.SetActive(false);
    }

    void GetLine()
    {
        
    }
}