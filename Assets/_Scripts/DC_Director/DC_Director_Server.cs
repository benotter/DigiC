using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_Director 
{
    public override void OnStartHost() 
    {
        
    }

    public override void OnStartServer()
    {

    }

    public override void OnServerConnect(NetworkConnection conn)
    {

    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject playerO = Instantiate(playerPrefab, Vector3.zero, Quaternion.Euler(Vector3.zero));
        DC_Player player = playerO.GetComponent<DC_Player>();

        NetworkServer.AddPlayerForConnection(conn, playerO, playerControllerId);
        game.AddPlayer(player);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {

    }
}