using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_Avatar_Chest_Chord : DC_Avatar_Part 
{
	public DC_Avatar_Chest chest;
	public override DC_Avatar.BodyParts GetBodyPart()
	{
		return DC_Avatar.BodyParts.ChestChord;
	}

	public override bool BoltStrike(DC_Bolt bolt) 
	{
		if(!chest.broken)
			return false;
        
        avatar.BoltStrike(this, bolt);
		return false;
	}
}
