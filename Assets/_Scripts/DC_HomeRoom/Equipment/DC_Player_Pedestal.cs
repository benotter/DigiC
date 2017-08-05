using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_Player_Pedestal : MonoBehaviour 
{
	public GameObject point;

	public Vector3 playerPoint;
	void Start () 
	{
		playerPoint = point.transform.position;

		Destroy(point);
	}
}
