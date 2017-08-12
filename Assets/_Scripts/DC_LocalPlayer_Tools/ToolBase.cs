using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ToolBase : MonoBehaviour 
{
	public Vector3 positionOffest = new Vector3(0f, 0f, 0f);
	public Vector3 rotationOffset = new Vector3(0f, 0f, 0f);
	
	protected SteamVR_TrackedController cont;

	public float trigger 
	{
		get { return cont ? cont.controllerState.rAxis1.x : 0f; }
	}
	
	[HideInInspector]
	public bool touch = false;
	[HideInInspector]
	public bool touchClick = false;
	public float touchX
	{
		get { return cont ? cont.controllerState.rAxis0.x : 0f; }
	}

	public float touchY
	{
		get { return cont ? cont.controllerState.rAxis0.y : 0f; }
	}

	[HideInInspector]
	public bool menuButton = false;
	[HideInInspector]
	public bool gripButton = false;

	public void SetController(SteamVR_TrackedController c)
	{
		cont = c;

		cont.MenuButtonClicked += MenuButtonClicked;
		cont.MenuButtonUnclicked += MenuButtonUnlicked;

		cont.SteamClicked += SteamClicked;

		cont.PadClicked += PadClicked;
		cont.PadUnclicked += PadUnclicked;

		cont.PadTouched += PadTouched;
		cont.PadUntouched += PadUntouched;

		cont.Gripped += Gripped;
		cont.Ungripped += Ungripped;
	}

	public void UnsetController()
	{
		cont.MenuButtonClicked -= MenuButtonClicked;
		cont.MenuButtonUnclicked -= MenuButtonUnlicked;

		cont.SteamClicked -= SteamClicked;

		cont.PadClicked -= PadClicked;
		cont.PadUnclicked -= PadUnclicked;

		cont.PadTouched -= PadTouched;
		cont.PadUntouched -= PadUntouched;

		cont.Gripped -= Gripped;
		cont.Ungripped -= Ungripped;

		cont = null;
	}

	private void MenuButtonClicked(object sender, ClickedEventArgs e) 
	{
		menuButton = true;	
	}
	private void MenuButtonUnlicked(object sender, ClickedEventArgs e) 
	{
		menuButton = false;
	}

	private void SteamClicked(object sender, ClickedEventArgs e) 
	{
		
	}

	private void PadClicked(object sender, ClickedEventArgs e) 
	{
		touchClick = true;
	}

	private void PadUnclicked(object sender, ClickedEventArgs e) 
	{
		touchClick = false;
	}

	private void PadTouched(object sender, ClickedEventArgs e) 
	{
		touch = true;
	}

	private void PadUntouched(object sender, ClickedEventArgs e) 
	{
		touch = false;
	}
	private void Gripped(object sender, ClickedEventArgs e) 
	{
		gripButton = true;
	}

	private void Ungripped(object sender, ClickedEventArgs e) 
	{
		gripButton = false;
	}
}
