using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_Avatar : NetworkBehaviour 
{
    private CapsuleCollider coll;

    public override void OnStartServer()
    {
        coll = GetComponent<CapsuleCollider>();
    }

	void ServerUpdate()
    {
        var mP = muzzle.transform.localPosition;
        coll.height = mP.y - (coll.radius * 2f);
        coll.center = new Vector3(mP.x, mP.y / 2f, mP.z);
    }
}
