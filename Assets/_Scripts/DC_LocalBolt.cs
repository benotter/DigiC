using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_LocalBolt : MonoBehaviour 
{
	public static int maxBoltLights = 20;
	public static int boltLightCount = 0;

	public LayerMask bustLayers;

	[Space(10)]

	public float preDamageTime = 0.1f;
	public float maxLifeTime = 10f;

	public int damage = 0;
	public float range = 0f;
	public float speed = 0f;
	
	public bool instant = false;
	public bool healthShot = false;

	[Space(10)]

	public GameObject playerO;
	public GameObject avatarO;

	[HideInInspector] public bool fired = false;
	[HideInInspector] public bool inMotion = false;

	[HideInInspector] public bool hit;

	private Light boltLight;
	private MeshRenderer rend;
	private Rigidbody rigid;

	private CapsuleCollider coll;

	private Vector3 startPos;

	private bool lightEnabled = false;
	private bool lightAdded = false;

	private float liveTime = 0f;

	private List<DC_Avatar_Part> ignoreParts = new List<DC_Avatar_Part>();

	void Start()
	{
		boltLight = GetComponent<Light>();
		rend = GetComponentInChildren<MeshRenderer>();

		rigid = GetComponent<Rigidbody>();

		coll = GetComponent<CapsuleCollider>();
	}

	public void AddLight() 
	{
		lightAdded = true;

		if(!boltLight)
			boltLight = GetComponent<Light>();

		if(boltLightCount + 1 > maxBoltLights)
			return;

		lightEnabled = true;

		if(boltLight)
			boltLight.enabled = true;

		boltLightCount++;
	}

	public void RemLight()
	{
		lightAdded = false;

		if(!boltLight)
			boltLight = GetComponent<Light>();

		if(boltLightCount - 1 < 0)
			return;

		lightEnabled = false;

		if(boltLight)
			boltLight.enabled = false;

		boltLightCount--;
	}

	void Update()
	{
		if(fired)
			liveTime += Time.deltaTime;

		if(fired && !instant && inMotion)
			UpdationMovement();

		if(fired && !instant && inMotion && Vector3.Distance(transform.position, startPos) > range)
			BustBolt(transform.position);

		if(fired && liveTime >= maxLifeTime)
			BustBolt(transform.position);
	}

	void UpdationMovement()
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

	public void FireInstant()
	{
		rend.enabled = false;
		
		if(lightAdded)
			RemLight();

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
				{
					Debug.Log("Part Hit!");
					part.ClientBoltStrike(this);
				}
		
				BustBolt(hit.point);
				break;
			}

			hit = true;
		}
		
		BustBolt(transform.position + (transform.forward * range));
	}

	public void FireSlow() 
	{
		if(!lightAdded)
			AddLight();

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
					part.ClientBoltStrike(this); 
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
		if(lightAdded)
			RemLight();

		Destroy(gameObject);
	}
}
