using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_Avatar_Muzzle_Chord : DC_Avatar_Part 
{
	public DC_Avatar_Muzzle muzzle;

	public override DC_Avatar.BodyParts GetBodyPart()
	{
		return DC_Avatar.BodyParts.MuzzleChord;
	}

	public override bool BoltStrike(DC_Bolt bolt) 
	{
		if(!muzzle.broken)
			return false;
        
        avatar.BoltStrike(this, bolt);
		return false;
	}
}
