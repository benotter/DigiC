using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_Game
{
    public NetworkClient gameOwner;
    
    public void AddPlayer(DC_Player player)
    {
        player.serverGameObject = gameObject;
    }
}
