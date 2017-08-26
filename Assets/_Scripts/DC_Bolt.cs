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
	[SyncVar] public bool hit = false;

	//Private Server-Side Variables

	[SyncVar] private bool inMotion = false;

	// Private Client-Side Variables

	
	private Renderer rend;
	private Collider coll;
	private Rigidbody rigid;
	private Light boltLight;

	private Vector3 startPos;
	
	private float bustTime = 0f;
	


	void Start()
	{
		rend = GetComponentInChildren<Renderer>();
		coll = GetComponent<Collider>();
		boltLight = GetComponent<Light>();

		if(boltLight)
		{
			if(!instant)
				if(boltLightCount + 1 > maxBoltLights)
					boltLight.enabled = false;
				else
					boltLightCount++;
			else 
			{
				if(rend) 
					rend.enabled = false;

				if(coll) 
					coll.enabled = false;

				if(boltLight) 
					boltLight.enabled = false;

			}
		}
	}

	void Update() 
	{
		if(isServer)
			ServerUpdate();
		else if(isClient)
			ClientUpdate();

		if(fired && !instant && inMotion)
			UpdateMotion();
	}

	void ServerUpdate() 
	{
		if(fired && !instant && inMotion && !hit && Vector3.Distance(startPos, transform.position) >= range)
			BustBolt(transform.position);
	}

	void ClientUpdate() 
	{
	
	}

	void UpdateMotion() 
	{
		rigid.position = rigid.position + transform.forward * (speed * Time.deltaTime);
	}

	void OnDestroy()
	{
		if(!instant && boltLight && boltLight.enabled)
			boltLightCount--;
	}

	public override void OnStartServer()
	{
		GetComponent<Light>().enabled = false;
	}
	
	public void Fire()
	{
		if(fired)
			return;
		else
			fired = true;

		startPos = transform.position;
		
		if(instant)
		{
			if(isServer) 
				ServerFireInstant();
			else if(isClient)
				ClientFireInstant();
		}
		else
		{
			if(isServer)
				ServerFireSlow();
			else if(isClient)
				ClientFireSlow();
		}
	}

	[Server] public void ServerFireInstant()
	{
		Ray ray = new Ray(transform.position, transform.forward);
		RaycastHit[] hits;		

		if(((hits = Physics.RaycastAll(ray, range, bustLayers)).Length ) > 0) 
		{
			foreach(RaycastHit hit in hits)
			{
				DC_Avatar_Part part;
				if((part = hit.collider.gameObject.GetComponent<DC_Avatar_Part>()) && !part.broken)	
				{
					Debug.Log("Part Hit!");
					part.BoltStrike(this);
					continue;
				}

				BustBolt(hit.point);
			}

			hit = true;
		}
		
		BustBolt(transform.position + (transform.forward * range));
	}

	[Server] public void ServerFireSlow()
	{
		inMotion = true;
	}


	[Client] public void ClientFireInstant() 
	{
		Ray ray = new Ray(transform.position, transform.forward);
		RaycastHit[] hits;		

		if(((hits = Physics.RaycastAll(ray, range, bustLayers)).Length ) > 0) 
		{
			foreach(RaycastHit hit in hits)
			{
				DC_Avatar_Part part;
				if((part = hit.collider.gameObject.GetComponent<DC_Avatar_Part>()) && !part.broken)	
				{
					Debug.Log("Part Hit!");
					part.BoltStrike(this);
					continue;
				}

				BustBolt(hit.point);
				break;
			}

			hit = true;
		}
		
		BustBolt(transform.position + (transform.forward * range));
	}

	[Client] public void ClientFireSlow() 
	{

	}


	void OnCollisionEnter(Collision col)
	{

	}

	[Server] public void BustBolt(Vector3 boltPos)
	{
		ServerBustBolt(boltPos);
		ClientBustBolt(boltPos);
	}

	[Server] public void ServerBustBolt(Vector3 pos) 
	{

	}

	[Client] public void ClientBustBolt(Vector3 pos) 
	{

	}

	// Server-Side Commands

	// Client-Side Commands

	[ClientRpc] public void RpcFire()
	{
		Fire();
	}

	[ClientRpc] public void RpcBustBolt(Vector3 boltPos)
	{

	}
}
