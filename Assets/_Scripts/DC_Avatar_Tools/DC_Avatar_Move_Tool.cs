using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_Avatar_Move_Tool : DC_Avatar_Tool_Base 
{
    public float maxMoveSpeed = 10f;
    public float maxMoveDistance = 1000f;

    private bool seeking = false;
    private bool moving = false;

    private bool canMove = false;

    private Vector3 movePoint = Vector3.zero;
    private float movePointDistance = 0f;

    private Vector3 targetPoint = Vector3.zero;

    private GameObject laser;
    private MeshRenderer laserRend;

    void Start()
    {
        
    }

    void Update()
    {
        if(trigger > 0.15f)
        {
            UpdateMovePoint();

            if(!seeking)
                seeking = true;

            if(trigger == 1f && canMove && !moving)
                moving = true;
        }
        else
        {
            if(seeking)
                seeking = false;

            if(moving)
                moving = false;
        }

        if(moving)
        {

        }
        else if(seeking)
        {

        }
    }

    void UpdateMovePoint()
    {
        var ht = hand.transform;

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
    }

    void UpdateTargetPoint()
    {

    }

    void CreateLaser()
    {
        var l = GameObject.CreatePrimitive(PrimitiveType.Cube);
        l.transform.localScale = new Vector3(0.01f, 0.01f, 0f);

        var rb = l.GetComponent<Rigidbody>();
        if(rb)
            Destroy(rb);

        var col = l.GetComponent<Collider>();

        if(col)
            Destroy(col);

        var mr = l.GetComponent<MeshRenderer>();

        l.transform.parent = transform;
        l.name = "Move Laser";

        laser = l;
        laserRend = mr;

        l.SetActive(false);
    }
}