using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_Director 
{
    public override void OnStartClient(NetworkClient client)
    {
        
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        ClientScene.Ready(conn);
        if(ClientScene.AddPlayer(0))
        {
            GameObject playerObj = ClientScene.localPlayers[0].gameObject;
            DC_Player player = playerObj.GetComponent<DC_Player>();
            if(player)
                game.homeRoom.SetRemotePlayer(player);
        }
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {

    }
}