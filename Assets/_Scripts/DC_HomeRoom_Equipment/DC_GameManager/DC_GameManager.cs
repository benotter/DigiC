using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_GameManager : MonoBehaviour 
{
	public DC_Director director;

	public bool gameInstalled = false;

	private bool newSphereRemoved = false;

	public void OnTriggerStay(Collider col)
	{
		if(gameInstalled)
			return;

		DC_JoinGame_LightSphere joinGame = col.gameObject.GetComponent<DC_JoinGame_LightSphere>();
		DC_NewGame_LightSphere newGame = col.gameObject.GetComponent<DC_NewGame_LightSphere>();

		// Debug.Log("TEST!");
		
		if( (joinGame && joinGame.inUse) || (newGame && newGame.inUse) ) 
			return;

		if(joinGame)
		{

		}
		else if(newGame)
		{
			newGame.Install(GetComponent<SphereCollider>().center);
			gameInstalled = true;

			director.StartGame();
		}
	}

	public void RemoveNewSphere()
	{

	}

	public void ReplaceNewSphere() 
	{
		
	}
}
