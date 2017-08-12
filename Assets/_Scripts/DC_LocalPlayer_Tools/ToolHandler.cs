using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ToolHandler : MonoBehaviour 
{
	public ToolBase startingTool;
	
	private ToolBase currentTool;
	private GameObject currentToolOldParent;
	private SteamVR_TrackedController cont;

	void Start()
	{
		cont = gameObject.GetComponent<SteamVR_TrackedController>();
		if(!cont)
			cont = gameObject.AddComponent<SteamVR_TrackedController>();

		if(startingTool)
			SetTool(startingTool);
	}

	void SetTool(ToolBase tool)
	{
		if(!tool.gameObject.activeSelf)
			tool.gameObject.SetActive(true);

		currentTool = tool;
		currentTool.SetController(cont);

		Transform toolT = tool.transform;
		
		toolT.localPosition = tool.positionOffest;
		toolT.localRotation = Quaternion.Euler(tool.rotationOffset);

		if(toolT.parent)
			currentToolOldParent = toolT.parent.gameObject;

		toolT.parent = transform;
	}

	void UnsetTool()
	{
		GameObject cTool = currentTool.gameObject;
		Transform cT = cTool.transform;

		cT.parent = null;

		if(currentToolOldParent)
		{
			cT.position = currentToolOldParent.transform.position;
			cT.parent = currentToolOldParent.transform;

			currentToolOldParent = null;
		}
		
		currentTool.UnsetController();
		currentTool = null;

		cTool.SetActive(false);
	}
}
