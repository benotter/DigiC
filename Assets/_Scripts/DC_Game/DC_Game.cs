using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_Game : NetworkBehaviour 
{
    public DC_HomeRoom homeRoom;

    [Space(10)]

    [SyncVar]
    public int gameMaxPlayers = 8;

    [SyncVar]
	public string gameName = "";

    [SyncVar]
    public string gameAddress = "";

    [SyncVar]
    public int gamePort = 0;

    [SyncVar]
    public int gamePlayerCount = 0;

    [SyncVar]
    public GameObject gameOwnerPlayerObj;
    
}
