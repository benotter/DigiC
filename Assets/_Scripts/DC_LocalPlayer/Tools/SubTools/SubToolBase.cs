using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SubToolBase : MonoBehaviour 
{
	public Vector3 positionOffset = new Vector3();
	public Vector3 rotationOffset = new Vector3();


	[Space(10)]
	public bool dropable = true;

	[Space(10)]
	public bool inUse = false;

	[Space(10)]

	public Color highlightColor = Color.red;

	private Vector3 originalPos;
	private Quaternion originalRot;
	private Transform originalPar;

	private Renderer rend;
	private Color originalColor;
}

public partial class SubToolBase 
{
	public void StartUse(PlayerTool pTool)
	{
		if(inUse)
			return;

		inUse = true;

		originalPos = gameObject.transform.localPosition;
		originalRot = gameObject.transform.localRotation;
		originalPar = gameObject.transform.parent;

		transform.parent = pTool.transform;

		transform.localPosition = positionOffset;
		transform.localRotation = Quaternion.Euler(rotationOffset);

		SetHighlight(false);
	}

	public void StopUse(PlayerTool pTool)
	{
		if(!inUse)
			return;

		transform.parent = originalPar;

		transform.localPosition = originalPos;
		transform.localRotation = originalRot;

		inUse = false;

		SetHighlight(false);
		pTool.SetSubtool(null);
	}

	public void SetHighlight(bool light = true)
	{
		if(!rend)
			rend = gameObject.GetComponent<MeshRenderer>();

		if(!rend)
			return;

		if(light && rend.material.color != highlightColor)
		{
			originalColor = rend.material.color;
			rend.material.color = highlightColor;
		}
		else if(rend.material.color != originalColor)
			rend.material.color = originalColor;
	}
}

public partial class SubToolBase 
{
	public float trigger = 0f;

	public float touchX = 0f;
	public float touchY = 0f;

	public bool touch = false;
	public bool touchClick = false;
	public bool menuButton = false;
	public bool gripButton = false;
}

public partial class SubToolBase 
{
	void OnTriggerEnter(Collider col)
	{
		if(inUse)
			return;

		PlayerTool pTool = col.gameObject.GetComponent<PlayerTool>();

		if(!pTool || pTool.subTool != null)
			return;

		SetHighlight(true);
		pTool.SetSubtool(this);
	}

	void OnTriggerStay(Collider col)
	{
		if(inUse)
			return;

		PlayerTool pTool = col.gameObject.GetComponent<PlayerTool>();

		if(!pTool || (pTool.subTool && (pTool.subTool == this || pTool.subTool.inUse) ) )
			return;

		SetHighlight(true);
		pTool.SetSubtool(this);
	}

	void OnTriggerExit(Collider col)
	{
		if(inUse)
			return;

		PlayerTool pTool = col.gameObject.GetComponent<PlayerTool>();
		
		if(!pTool)
			return;

		SetHighlight(false);
		pTool.SetSubtool(null);
	}
}
