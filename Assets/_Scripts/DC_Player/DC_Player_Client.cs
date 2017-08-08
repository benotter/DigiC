using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_Player
{
    public override void OnStartLocalPlayer()
    {


    }
    
    [ClientRpc]
    public void RpcSetAvatarSpawn(GameObject aS)
    {
        avatarSpawn = aS;
        var avaS = aS.GetComponent<DC_Avatar_Spawn>();
        if(avaS)
            avaS.SetPlayer(gameObject);

        var p = transform.position;
        aS.transform.position = new Vector3(p.x, gameGrid.transform.position.y + 1f, p.z);
        avaS.Lock();
    }

    [ClientRpc]
    public void RpcSetAvatar(GameObject a)
    {
        avatar = a;
        var ava = a.GetComponent<DC_Avatar>();
        if(ava)
            serverGame.homeRoom.avatarSync.SetAvatar(ava);
    }
}