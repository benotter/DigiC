using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTool : ToolBase 
{
	public enum Hand
	{
		None,
		Right,
		Left
	}
	public Hand hand = Hand.Right;

	[Space(10)]

	public bool overrideTriggerDown = false;
	public float triggerDownMin = 0.15f;
	private float lastTriggerDownMin = 0.15f;

	[Space(10)]

	public SubToolBase subTool;

	private float triggerDownTime = 0f;
	private float gripDownTime = 0f;

	void Start()
	{
		if(subTool)
			subTool.StartUse(this);	
	}
	
	public void SetSubtool(SubToolBase tool)
	{
		if(subTool && subTool != tool)
			subTool.StopUse(this);
		
		subTool = tool;

		if(tool && !overrideTriggerDown)
		{
			lastTriggerDownMin = triggerDownMin;
			triggerDownMin = tool.triggerDownMin;
		}
		else if(!overrideTriggerDown) 
		{
			triggerDownMin = lastTriggerDownMin;
		}		
	}

	void Update () 
	{		
		if(subTool && !subTool.inUse && trigger >= triggerDownMin && triggerDownTime <= 0)
		{
			if(subTool.isButton)
				subTool.OnPressHandler(true);
			else
				subTool.StartUse(this);				
		}
		else if(subTool && (subTool.noHold || subTool.isButton) && trigger < triggerDownMin)
		{
			if(subTool.isButton)
				subTool.OnPressHandler(false);
		
			if(subTool.noHold)
				subTool.StopUse(this);
		}
		else if(subTool && !subTool.noHold && gripButton && subTool.dropable && gripDownTime <= 0)
			subTool.StopUse(this);

		if(subTool)
			UpdateSubtoolState();

		if(gripButton)
			gripDownTime += Time.deltaTime;
		
		if(trigger >= triggerDownMin)
			triggerDownTime += Time.deltaTime;

		if(!gripButton && gripDownTime > 0)
			gripDownTime = 0;
		
		if(trigger < triggerDownMin && triggerDownTime > 0)
			triggerDownTime = 0;
	}

	void UpdateSubtoolState()
	{
		subTool.trigger = trigger;
		subTool.menuButton = menuButton;
		subTool.gripButton = gripButton;
		subTool.touchClick = touchClick;
		subTool.touch = touch;
		subTool.touchX = touchX;
		subTool.touchY = touchY;
	}
}
