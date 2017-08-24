using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DC_Bolt : NetworkBehaviour
{
	public static int maxBoltLights = 0;
	public static int boltLightCount = 0;

	// Client-Side Variables
	public LayerMask bustLayers;

	[Space(10)]

	[HideInInspector] public Light boltLight;

	[Space(10)]

	// Server-Side Variables
	[SyncVar] public GameObject playerO;
	[SyncVar] public GameObject avatarO;

	[Space(10)]

	[SyncVar] public bool instant = true;
	[SyncVar] public bool healthShot = false;

	[Space(10)]

	[SyncVar] public int damage = 10;
	[SyncVar] public float speed = 10f;
	[SyncVar] public float range = 100f;
	
	[Space(10)]

	[SyncVar] public bool fired = false;	

	//Private Server-Side Variables
	[SyncVar] private bool inMotion = false;

	// Private Client-Side Variables

	private Renderer rend;
	private Collider coll;

	void Start()
	{
		rend = GetComponentInChildren<Renderer>();

		coll = GetComponent<Collider>();
		boltLight = GetComponent<Light>();

		if(boltLight)
		{
			if(boltLightCount + 1 > maxBoltLights)
				boltLight.enabled = false;
			else
				boltLightCount++;
		}
	}

	void OnDestroy()
	{
		if(boltLight && boltLight.enabled)
			boltLightCount--;
	}

	public override void OnStartServer()
	{
		GetComponent<Light>().enabled = false;
	}
	
	[Server] public void Fire()
	{
		if(fired)
			return;

		if(instant)
			FireInstant();
		else
			FireSlow();

		fired = true;
	}

	public void FireInstant()
	{
		if(rend) rend.enabled = false;
		if(coll) coll.enabled = false;
		if(boltLight) boltLight.enabled = false;

		Ray ray = new Ray(transform.position, transform.forward);
		RaycastHit[] hits;		

		if(((hits = Physics.RaycastAll(ray, range, bustLayers)).Length ) > 0)
			foreach(RaycastHit hit in hits)
				{
					DC_Avatar_Part part;
					if((part = hit.collider.gameObject.GetComponent<DC_Avatar_Part>()) && !part.broken)	
					{
						part.BoltStrike(this);
						continue;
					}	
					BustBolt(hit.point);
				}

		BustBolt(transform.position + (transform.forward * range));
	}

	public void FireSlow()
	{

	}

	[Server] void OnCollisionEnter(Collision col)
	{

	}

	[Server] public void BustBolt(Vector3 boltPos)
	{
		RpcBustBolt(boltPos);
		NetworkServer.Destroy(gameObject);
	}

	// Server-Side Commands

	// Client-Side Commands

	[ClientRpc] public void RpcBustBolt(Vector3 boltPos)
	{

	}
}
