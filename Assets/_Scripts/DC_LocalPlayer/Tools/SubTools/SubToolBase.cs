using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SubToolBase : MonoBehaviour 
{
	public Vector3 positionOffset = new Vector3();
	public Vector3 rotationOffset = new Vector3();


	[Space(10)]
	
	public bool isButton = false;
	public bool dropable = true;
	public bool noHold = false;

	[Space(10)]

	public bool buttonEnabled = true;
	public bool snapBack = true;
	public bool smoothSnapBack = true;
	public float snapBackSpeed = 0.2f;

	[Space(10)]

	public float triggerDownMin = 0.15f;

	[Space(10)]

	public bool inUse = false;

	[Space(10)]

	public Color highlightColor = Color.red;
	public Color buttonClickColor = Color.blue;

	private Vector3 originalPos;
	private Quaternion originalRot;
	private Transform originalPar;

	private Renderer rend;
	private Color originalColor;

	private bool snappingBack = false;
	private float snappingBackTime = 0f;

	private bool buttonDown = false;
}

public partial class SubToolBase 
{
	void Update()
	{
		UpdateSnapping();
	}
	
	public void SetButtonEnabled(bool enale)
	{
		buttonEnabled = enabled;
	}

	public void OnPressHandler(bool down)
	{
		if(down)
		{
			buttonDown = true;
			OnPress();
			SetHighlight(true);
		}
		else if(buttonDown)
		{
			buttonDown = false;
			OnRelease();
			SetHighlight(true);
		}
	}

	public virtual void OnPress() {}

	public virtual void OnRelease() {}

	public void StartUse(PlayerTool pTool)
	{
		if(inUse)
			return;

		inUse = true;

		if(!snappingBack)
		{
			originalPos = gameObject.transform.localPosition;
			originalRot = gameObject.transform.localRotation;
			originalPar = gameObject.transform.parent;
		}
		else
		{
			
			snappingBack = false;
			snappingBackTime = 0f;
		}

		Debug.Log("Starting Tool Use");
		
		transform.parent = pTool.transform;

		transform.localPosition = positionOffset;
		transform.localRotation = Quaternion.Euler(rotationOffset);

		SetHighlight(false);
	}

	public void StopUse(PlayerTool pTool)
	{
		if(!inUse)
			return;

		inUse = false;

		transform.parent = originalPar;

		if(snapBack && !smoothSnapBack)
		{
			transform.localPosition = originalPos;
			transform.localRotation = originalRot;
		}

		if (snapBack && smoothSnapBack)
		{
			snappingBack = true;
			Debug.Log("Stop Use");
		}

		SetHighlight(false);
		pTool.SetSubtool(null);
	}

	public void UpdateSnapping()
	{
		if(snappingBack)
		{
			Debug.Log("Snapping!");
			if(snappingBackTime < snapBackSpeed)
			{
				snappingBackTime += Time.deltaTime;

				float prog = snappingBackTime / snapBackSpeed;
				transform.localPosition = Vector3.Lerp(transform.localPosition, originalPos, prog);
				transform.localRotation = Quaternion.Lerp(transform.localRotation, originalRot, prog);
			}
			else
			{
				snappingBack = false;
				snappingBackTime = 0f;
			}
		}
	}

	public void SetHighlight(bool light = true)
	{
		if(!rend)
			rend = gameObject.GetComponent<MeshRenderer>();

		if(!rend)
			return;

		Color c = highlightColor;

		if(isButton && buttonDown)
			c = buttonClickColor;

		if(light && rend.material.color != c)
		{
			Color curC = rend.material.color;
			
			if(curC != buttonClickColor && curC != highlightColor)
				originalColor = rend.material.color;
			
			rend.material.color = c;
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

		if(isButton && !buttonEnabled)
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

		if(isButton && !buttonEnabled)
			return;

		PlayerTool pTool = col.gameObject.GetComponent<PlayerTool>();

		if(!pTool || (pTool.subTool) )
			return;

		SetHighlight(true);
		pTool.SetSubtool(this);
	}

	void OnTriggerExit(Collider col)
	{
		if(inUse)
			return;

		if(isButton && !buttonEnabled)
			return;

		PlayerTool pTool = col.gameObject.GetComponent<PlayerTool>();
		
		if(!pTool)
			return;

		SetHighlight(false);
		pTool.SetSubtool(null);
	}
}
