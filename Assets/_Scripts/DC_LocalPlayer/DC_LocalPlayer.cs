using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_LocalPlayer : MonoBehaviour 
{
	public GameObject HMD;
	public GameObject rightController;
	public GameObject leftController;

	public Vector3 hmdPos 
	{
		get { return transform.InverseTransformDirection(HMD.transform.position); }
	}

	public Quaternion hmdRot 
	{
		get { return HMD.transform.rotation; }
	}

	public Vector3 rightPos 
	{
		get { return transform.InverseTransformDirection(rightController.transform.position); }
	}

	public Quaternion rightRot 
	{
		get { return rightController.transform.rotation; }
	}

	public Vector3 leftPos 
	{
		get { return transform.InverseTransformDirection(leftController.transform.position); }
	}

	public Quaternion leftRot 
	{
		get { return leftController.transform.rotation; }
	}

	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}
