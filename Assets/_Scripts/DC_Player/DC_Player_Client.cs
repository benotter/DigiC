using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_Player
{
    [ClientRpc]
    public void RpcSetAvatar(GameObject a)
    {
        avatar = a;
        var ava = a.GetComponent<DC_Avatar>();
        if(ava)
            serverGame.homeRoom.avatarSync.SetAvatar(ava);
    }
}