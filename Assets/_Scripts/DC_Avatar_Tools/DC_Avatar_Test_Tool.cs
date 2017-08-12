using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_Avatar_Test_Tool : DC_Avatar_Tool_Base 
{
    public Color laserOne = Color.blue;
    public Color laserTwo = Color.red;

    public float maxLaserDistance = 1000f;

    private GameObject laser;
    private MeshRenderer laserRend;

    private bool laserOn = false;

    void Start()
    {
        
    }

    void Update()
    {
        if(!laser)
            CreateLaser();

        if(trigger > 0.15f)
        {
            if(!laserOn)
            {
                laserOn = true;
                laser.SetActive(true);
            }

            UpdateLaser();
            UpdateLaserColor();
        }
        else if(laserOn)
        {
            laserOn = false;
            laser.SetActive(false);
        }
    }

    void CreateLaser()
    {
        var l = GameObject.CreatePrimitive(PrimitiveType.Cube);
        l.transform.localScale = new Vector3(0.1f, 0.1f, 0f);

        var rb = l.GetComponent<Rigidbody>();
        if(rb)
            Destroy(rb);

        var col = l.GetComponent<Collider>();

        if(col)
            Destroy(col);

        var mr = l.GetComponent<MeshRenderer>();

        l.transform.parent = transform;
        l.name = "Test Laser";

        laser = l;
        laserRend = mr;

        l.SetActive(false);
    }

    void UpdateLaser()
    {
        var lt = laser.transform;
        var ht = hand.transform;

        Ray ray = new Ray(ht.position, ht.forward);
        RaycastHit hit;

        lt.localRotation = ht.rotation;

        if(Physics.Raycast(ray, out hit, maxLaserDistance))
        {
            lt.localScale = new Vector3(lt.localScale.x, lt.localScale.y, hit.distance);
            lt.localPosition = ht.position + (ht.forward * (hit.distance / 2f));
        }
        else
        {
            lt.localScale = new Vector3(lt.localScale.x, lt.localScale.y, maxLaserDistance);
            lt.localPosition = ht.position + (ht.forward * (maxLaserDistance / 2f));
        }
    }

    void UpdateLaserColor()
    {
        Color c;

        if(trigger < 1f)
            c = laserOne;
        else
            c = laserTwo;

        Color oldC = laserRend.material.color;

        if(oldC != c)
            laserRend.material.color = c;
    }
}