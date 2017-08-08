using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_Game
{
    public NetworkClient gameOwner;
    
    public override void OnStartServer()
    {
        gameGrid.gridCellSize = gameGridSize;
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
        if(!player.avatar && player.avatarSpawn)
        {
            var aS = player.avatarSpawn.GetComponent<DC_Avatar_Spawn>().lockedIn;

            if(aS)
            {
                GameObject avatar = Instantiate(avatarPrefab);
                NetworkServer.SpawnWithClientAuthority(avatar, player.gameObject);

                player.RpcSetAvatar(avatar);
                avatar.transform.position = player.avatarSpawn.transform.position;
            }
        }
    }

    public void AddPlayer(DC_Player player)
    {
        player.serverGameObject = gameObject;
        player.serverGame = this;

        if(player.gameObject == gameOwner.connection.playerControllers[0].gameObject)
            gameOwnerPlayerObj = player.gameObject;

        RpcPlayerJoined(player.gameObject);
        gamePlayerCount++;
    }

    public void RemPlayer(DC_Player player)
    {
        RpcPlayerLeft(player.gameObject);
        gamePlayerCount--;
    }
}
