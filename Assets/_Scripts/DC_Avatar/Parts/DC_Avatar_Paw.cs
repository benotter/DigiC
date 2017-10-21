using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_Avatar_Paw : DC_Avatar_Part 
{
	public enum Paw 
	{
		RightPaw,
		LeftPaw
	}

	public Paw pawHand = Paw.RightPaw;

	public override DC_Avatar.BodyParts GetBodyPart()
	{
		if(pawHand == Paw.RightPaw)
			return DC_Avatar.BodyParts.RightPaw;
		else
			return DC_Avatar.BodyParts.LeftPaw;
	}
}
