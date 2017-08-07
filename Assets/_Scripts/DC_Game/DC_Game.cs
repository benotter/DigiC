using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_Game : NetworkBehaviour 
{
    public DC_HomeRoom homeRoom;
    // public DC_GameRoom gameRoom;

    [Space(10)]

    public GameObject remotePlayer;

    [Space(10)]

    public GameObject avatarPrefab;

    [Space(10)]

    public HashSet<GameObject> players = new HashSet<GameObject>();

    [Space(10)]
    

    [SyncVar]
	public string gameName = "";

    [SyncVar]
    public int gameMaxPlayers = 8;

    [SyncVar]
    public string gameAddress = "";

    [SyncVar]
    public int gamePort = 0;

    [SyncVar]
    public int gamePlayerCount = 0;

    [SyncVar]
    public GameObject gameOwnerPlayerObj;

    [SyncVar]
    public int gameGridSize = 20;

    void Start()
    {

    }


}
