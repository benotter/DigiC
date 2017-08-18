using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SubToolBase : MonoBehaviour 
{
	public Vector3 positionOffset = new Vector3();
	public Vector3 rotationOffset = new Vector3();

	public bool grabAsIs = false;


	[Space(10)]
	public bool toolEnabled = true;

	[Space(10)]
	
	public bool isButton = false;
	public bool dropable = true;
	public bool noHold = false;

	[Space(10)]
	
	public bool snapBack = true;
	public bool smoothSnapBack = true;
	public float snapBackSpeed = 0.2f;

	[Space(10)]

	public float triggerDownMin = 0.15f;

	[Space(10)]

	public bool inUse = false;

	[Space(10)]

	public Color highlightColor = Color.red;

	[Space(5)]

	public Color buttonClickColor = Color.blue;
	public Color buttonDisabledColor = Color.gray;


	[HideInInspector]
	public PlayerTool.Hand hand = PlayerTool.Hand.None;
	
	protected Vector3 originalPos;
	protected Quaternion originalRot;
	protected Transform originalPar;

	private Renderer rend;
	private Color originalColor;

	private bool snappingBack = false;
	private float snappingBackTime = 0f;

	private bool buttonDown = false;

	private bool hasHover = false;
}

public partial class SubToolBase 
{
	void Update()
	{
		if(smoothSnapBack)
			UpdateSnapping();

		CheckHighlight();
	}
	
	public void SetToolEnabled(bool en)
	{
		toolEnabled = en;
	}

	public void SetOrigin(Vector3 oPos, Quaternion oRot, Transform oPar = null)
	{
		originalPos = oPos;
		originalRot = oRot;

		if(oPar)
			originalPar = oPar;
	}

	public void OnPressHandler(bool down)
	{
		if(down)
		{
			buttonDown = true;
			OnPress();
		}
		else if(buttonDown)
		{
			buttonDown = false;
			OnRelease();
		}
	}

	public virtual void OnPress() {}

	public virtual void OnRelease() {}

	public void StartUse(PlayerTool pTool)
	{
		if(inUse)
			return;

		inUse = true;

		hand = pTool.hand;

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
		
		transform.parent = pTool.transform;

		if(!grabAsIs)
		{
			transform.localPosition = positionOffset;
			transform.localRotation = Quaternion.Euler(rotationOffset);
		}

		var coll = GetComponent<Collider>();
		coll.isTrigger = false;
	}

	public void StopUse(PlayerTool pTool)
	{
		if(!inUse)
			return;

		inUse = false;

		hand = PlayerTool.Hand.None;

		transform.parent = originalPar;

		if(snapBack && !smoothSnapBack)
		{
			transform.localPosition = originalPos;
			transform.localRotation = originalRot;
		}

		if (snapBack && smoothSnapBack)
			snappingBack = true;

		buttonDown = false;

		pTool.SetSubtool(null);

		var coll = GetComponent<Collider>();
		coll.isTrigger = true;
	}

	public void UpdateSnapping()
	{
		if(snappingBack)
		{
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

	public void CheckHighlight()
	{			
		if(!rend)
			rend = gameObject.GetComponent<MeshRenderer>();

		if(!rend)
			return;
		
		bool light = false;

		Color c = highlightColor;

		if(!isButton)
		{
			if(hasHover)
				light = true;

			if(!toolEnabled || inUse)
				light = false;
		}

		if(isButton)
		{
			if(hasHover)
				light = true;

			if(!toolEnabled)
			{
				light = true;
				c = buttonDisabledColor;
			}

			if(buttonDown)
			{
				light = true;
				c = buttonClickColor;
			}
		}

		Color curC = rend.material.color;
		if(light && curC != c)
		{	
			if( curC != highlightColor &&
				curC != buttonClickColor && 
				curC != buttonDisabledColor )
			{
				originalColor = curC;
			}
			
			rend.material.color = c;
		}
		else if(!light && curC != originalColor && originalColor != Color.clear)
		{
			rend.material.color = originalColor;
		}
			
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

		if(!toolEnabled)
			return;

		PlayerTool pTool = col.gameObject.GetComponent<PlayerTool>();

		if(!pTool || pTool.subTool != null)
			return;

		pTool.SetSubtool(this);
		hasHover = true;
	}

	void OnTriggerStay(Collider col)
	{
		if(inUse)
			return;

		if(!toolEnabled)
			return;

		PlayerTool pTool = col.gameObject.GetComponent<PlayerTool>();

		if(!pTool || (pTool.subTool) )
			return;

		pTool.SetSubtool(this);
		hasHover = true;
	}

	void OnTriggerExit(Collider col)
	{
		if(inUse)
			return;

		hasHover = false;
		buttonDown = false;

		PlayerTool pTool = col.gameObject.GetComponent<PlayerTool>();
		
		if(!pTool || pTool.subTool != this)
			return;

		pTool.SetSubtool(null);
	}
}
