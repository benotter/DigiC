using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_Avatar_Chest : DC_Avatar_Part 
{
	public Color playerColor = Color.blue;
	public Color brokenColor = Color.red * new Color(1f,1f,1f,0.5f);

	private MeshRenderer rend;

	void Start () 
	{
		rend = GetComponent<MeshRenderer>();
		UpdateColor();
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	void UpdateColor()
	{
		Color c;

		if(!broken)
			c = playerColor;
		else
			c = brokenColor;

		Color curC = rend.material.color;

		if(curC != c)
			rend.material.color = c;
	}

	public override DC_Avatar.BodyParts GetBodyPart()
	{
		return DC_Avatar.BodyParts.Chest;
	}

	public override void OnBreak()
	{
		UpdateColor();
	}
}
