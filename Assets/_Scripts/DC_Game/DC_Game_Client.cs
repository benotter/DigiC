using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_Game
{   
    private bool setupGameRoomSinceLastRound = false;
    
    [ClientRpc]
    public void RpcSetupGameRoom()
    {
        if(!setupGameRoomSinceLastRound)
        {
            setupGameRoomSinceLastRound = true;
            // gameRoom.SetupGameRoom();
        }
    }
    
    [ClientRpc]
    public void RpcPlayerJoined(GameObject player)
    {
        if(!players.Contains(player))
            players.Add(player);
    }

    [ClientRpc]
    public void RpcPlayerLeft(GameObject player)
    {
        if(players.Contains(player))
            players.Remove(player);
    }

}
