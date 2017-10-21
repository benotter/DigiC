using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DC_Bolt : NetworkBehaviour
{
	public LayerMask bustLayers;

	[Space(10)]

	[SyncVar] public GameObject playerO;
	[SyncVar] public GameObject avatarO;

	[Space(10)]

	[SyncVar] public bool instant = true;
	[SyncVar] public bool healthShot = false;

	[Space(10)]

	[SyncVar] public float preDamageTime = 0.1f;
	[SyncVar] public float maxLifeTime = 10f;

	[SyncVar] public int damage = 10;
	[SyncVar] public float speed = 10f;
	[SyncVar] public float range = 100f;
	
	[Space(10)]

	[SyncVar] public bool fired = false;
	[SyncVar] public bool hit = false;

	//Private Server-Side Variables

	[SyncVar] private bool inMotion = false;

	// Private Client-Side Variables
	private CapsuleCollider coll;
	private Rigidbody rigid;

	private Vector3 startPos;
	private float bustTime = 0f;

	private float liveTime = 0f;

	private List<DC_Avatar_Part> ignoreParts = new List<DC_Avatar_Part>();

	void Start()
	{
		coll = GetComponent<CapsuleCollider>();
		rigid = GetComponent<Rigidbody>();
	}

	public override void OnStartServer()
	{
		
	}

	void Update() 
	{
		if(fired)
			liveTime += Time.deltaTime;

		if(fired && !instant && inMotion)
		{
			UpdateIgnoreColliders();
			UpdateMovement();
		}

		if(fired && !instant && inMotion && !hit && Vector3.Distance(startPos, transform.position) >= range)
			BustBolt(transform.position);

		if(fired && liveTime >= maxLifeTime)
			BustBolt(transform.position);			
	}

	void UpdateMovement() 
	{
		rigid.position = rigid.position + transform.forward * (speed * Time.deltaTime);
	}

	void UpdateIgnoreColliders() 
	{		
		Vector3 point1 = (coll.height / 2f ) * Vector3.forward;
		Vector3 point2 = (coll.height / 2f) * Vector3.back;

		float jump = (speed * Time.deltaTime);

		RaycastHit[] hits = Physics.CapsuleCastAll(point1, point2, coll.radius, transform.forward, jump, bustLayers);

		if(hits.Length > 0)
			foreach(RaycastHit hit in hits)
			{
				if(hit.collider == coll)
					continue;
					
				DC_Avatar_Part part = hit.collider.gameObject.GetComponent<DC_Avatar_Part>();
				if(part && part.broken && !ignoreParts.Contains(part))
				{
					Physics.IgnoreCollision(hit.collider, coll);
					ignoreParts.Add(part);
				}
			}
	}

	public void Fire()
	{
		if(fired)
			return;
		else
			fired = true;

		startPos = transform.position;
		
		if(instant)
			FireInstant();
		
		else
			FireSlow();
	}

	[Server] public void FireInstant()
	{
		Ray ray = new Ray(transform.position, transform.forward);
		RaycastHit[] hits;		

		if(((hits = Physics.RaycastAll(ray, range, bustLayers)).Length ) > 0) 
		{
			foreach(RaycastHit hit in hits)
			{
				DC_Avatar_Part part = hit.collider.gameObject.GetComponent<DC_Avatar_Part>();

				if(part && part.broken)	
					continue;
				else if(part)
					part.BoltStrike(this);
		
				BustBolt(hit.point);
				break;
			}

			hit = true;
		}
		
		BustBolt(transform.position + (transform.forward * range));
	}

	public void FireSlow()
	{
		inMotion = true;
	}

	void OnTriggerEnter(Collider col)
	{
		if(hit)
			return;
			
		GameObject colO = col.gameObject;
		if(colO && (bustLayers.value | (1 << colO.layer)) == bustLayers.value) 
		{
			DC_Avatar_Part part = colO.GetComponent<DC_Avatar_Part>();

			if(part)
			{
				if(!part.broken)
					part.BoltStrike(this);
				else 
				{
					Debug.Log("Skpping Part: " + col.gameObject.name);
					return;
				}
			}
			
			hit = true;
				
			BustBolt(transform.position);
		}
	}

	public void BustBolt(Vector3 pos) 
	{
		NetworkServer.Destroy(gameObject);
	}
}
