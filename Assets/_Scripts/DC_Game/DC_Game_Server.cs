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

    public void RequestAvatarSpawn(DC_Player player)
    {
        if(!player.avatarSpawn)
        {
            GameObject avatarSpawn = Instantiate(avatarSpawnPrefab);
            NetworkServer.SpawnWithClientAuthority(avatarSpawn, player.gameObject);

            player.RpcSetAvatarSpawn(avatarSpawn);
        }
    }
    
    public void RequestAvatar(DC_Player player)
    {
        if(!player.avatar) // && player.avatarSpawn)
        {
            // var aS = player.avatarSpawn.GetComponent<DC_Avatar_Spawn>().lockedIn;

            if(true) // aS)
            {
                GameObject avatar = Instantiate(avatarPrefab);
                NetworkServer.SpawnWithClientAuthority(avatar, player.gameObject);

                player.RpcSetAvatar(avatar);
            }
        }
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
}
