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

        RpcPlayerJoined(player.gameObject);
    }

    public void RemPlayer(DC_Player player)
    {
        RpcPlayerLeft(player.gameObject);
    }

    public void RequestAvatar(DC_Player player)
    {
        if(!player.avatar)
        {
            GameObject avatar = Instantiate(avatarPrefab);
            NetworkServer.SpawnWithClientAuthority(player.gameObject, avatar);

            player.avatar = avatar;
        }
    }
}
