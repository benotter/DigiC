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

	[Space(10)]

	// Server-Side Variables
	[SyncVar] public BlasterType blasterType = BlasterType.SingleFire;

	[Space(10)]

	[SyncVar] public bool bulletStyleBolt = true;
	[SyncVar] public bool boltHeals = false;

	[SyncVar] public int damage = 10;
	[SyncVar] public float range = 100f;
	
	[Space(10)]

	[SyncVar] public int energyMax = 100;
	[SyncVar] public int energyChargeRate = 10;

	[Space(10)]

	[SyncVar] public float fireRate = 1f;

	[Space(10)]

	[SyncVar] public bool firing = false;

	// Private Server-Side Variables

	// Private Client-Side Variables

	private bool fired = false;

	private float lastFireTime = 0f;

	void Update() 
	{
		ServerUpdate();
		ClientUpdate();
	}

	[Server] void ServerUpdate() 
	{
		lastFireTime += Time.deltaTime;

		if(firing) 
		{	
			if(lastFireTime > 1 / fireRate)
				HandleBlasterFire();
		}
		else 
		{
			if(lastFireTime > 1 / fireRate && fired)
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
				if(!fired)
				{
					Fire();
					fired = true;
				}
					
			break;

			case BlasterType.SemiAutoFire:
			break;

			case BlasterType.AutoFire:
			break;

			case BlasterType.ChargeFire:
			break;
		}
	}

	[Server] public void Fire()
	{
		GameObject boltO = Instantiate(boltPrefab, firePoint.position, firePoint.rotation);
		DC_Bolt bolt = boltO.GetComponent<DC_Bolt>();

		bolt.damage = damage;
		bolt.range = range;

		bolt.instant = bulletStyleBolt;
		bolt.healthShot = boltHeals;

		bolt.playerO = avatar.playerO;
		bolt.avatarO = avatar.gameObject;

		NetworkServer.Spawn(boltO);

		bolt.Fire();
		bolt.RpcFire();
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
}
