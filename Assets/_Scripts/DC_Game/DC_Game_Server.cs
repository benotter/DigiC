using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_Game
{
    public NetworkClient gameOwner;
    
    public override void OnStartServer()
    {
        
    }
    public void AddPlayer(DC_Player player)
    {
        player.serverGameObject = gameObject;
        player.serverGame = this;

        if(player.connectionToClient == gameOwner.connection)
            gameOwnerPlayerObj = player.gameObject;
    }

    public void RequestAvatar(DC_Player player)
    {
        
    }
}
