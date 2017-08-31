using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public partial class DC_Avatar_Blaster : DC_Avatar_Tool
{	
	// Client-Side Variables

	[Space(10)]

	public Transform firePoint;

	public GameObject boltPrefab;
	public GameObject localBoltPrefab;

	[Space(10)]

	// Server-Side Variables
	[SyncVar] public BlasterType blasterType = BlasterType.SingleFire;

	[Space(10)]

	[SyncVar] public bool bulletStyleBolt = true;
	[SyncVar] public bool boltHeals = false;

	[SyncVar] public float preDamageTime = 0.1f;
	[SyncVar] public float maxLifeTime = 10f;

	[SyncVar] public int damage = 10;
	[SyncVar] public float range = 100f;
	[SyncVar] public float speed = 10f;
	
	[Space(10)]

	[SyncVar] public int energyMax = 100;
	[SyncVar] public int energyChargeRate = 10;

	[Space(10)]

	[SyncVar] public int fireRate = 1;

	[Space(10)]

	[SyncVar] public bool firing = false;

	// Private Server-Side Variables

	// Private Client-Side Variables

	private bool fired = false;
	private float lastFireTime = 0f;
	

	void Update() 
	{
		if(isServer) 
			ServerUpdate();

		if(isClient)
			ClientUpdate();
	}

	[Server] void ServerUpdate() 
	{
		lastFireTime += Time.deltaTime;

		bool canFire = lastFireTime >= 1f / (float) fireRate;

		if(firing) 
		{	
			if(canFire)
				HandleBlasterFire();
		}
		else 
		{
			if(fired && canFire)
				fired = false;
		}
	}

	[Client] void ClientUpdate()
	{
		if(trigger == 1)
		{
			if(!firing)
				CmdStartFiring();
		}
		else if(firing)
			CmdStopFiring();
	}

	[Server] void HandleBlasterFire()
	{
		switch(blasterType) 
		{
			case BlasterType.SingleFire:
			case BlasterType.SemiAutoFire:
				if(!fired)
					Fire();
			break;

			case BlasterType.AutoFire:
				Fire();
			break;

			case BlasterType.ChargeFire:
			break;
		}
	}

	[Server] public void Fire()
	{
		RpcCreateClientBolt();

		GameObject boltO = Instantiate(boltPrefab, firePoint.position, firePoint.rotation);
		DC_Bolt bolt = boltO.GetComponent<DC_Bolt>();

		bolt.preDamageTime = preDamageTime;

		bolt.damage = damage;
		bolt.range = range;
		bolt.speed = speed;

		bolt.instant = bulletStyleBolt;
		bolt.healthShot = boltHeals;

		bolt.playerO = avatar.playerO;
		bolt.avatarO = avatar.gameObject;

		NetworkServer.Spawn(boltO);
		bolt.Fire();

		lastFireTime = 0f;
		fired = true;
	}

	// Server-Side Commands

	[Command] public void CmdStartFiring()
	{
		firing = true;
	}

	[Command] public void CmdStopFiring() 
	{
		firing = false;
	}

	// Client-Side Commands

	[ClientRpc] public void RpcCreateClientBolt()
	{
		GameObject newLocalB = Instantiate(localBoltPrefab, firePoint.position, firePoint.rotation);
		DC_LocalBolt lBolt = newLocalB.GetComponent<DC_LocalBolt>();

		lBolt.preDamageTime = preDamageTime;

		lBolt.damage = damage;
		lBolt.range = range;
		lBolt.speed = speed;

		lBolt.instant = bulletStyleBolt;
		lBolt.healthShot = boltHeals;

		lBolt.playerO = playerO;
		lBolt.avatarO = avatarO;

		lBolt.Fire();
	}
}

public partial class DC_Avatar_Blaster 
{
	public enum BlasterType 
	{
		SingleFire,
		
		SemiAutoFire,
		AutoFire,

		ChargeFire,
	}

	[System.Serializable] public struct LocalBolt
	{
		public float damage;
		public float range;
		public bool instant;
		public bool healthShot;

		public LocalBolt Setup(float d, float r, bool inst, bool heal)
		{
			damage = d;
			range = r;
			instant = inst;
			healthShot = heal;

			return this;
		}
	}
}
