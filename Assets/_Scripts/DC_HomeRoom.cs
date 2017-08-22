using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_HomeRoom : MonoBehaviour 
{
    public DC_LocalPlayer localPlayer;

    [Space(10)]

    public DC_GameGrid gameGrid;
    public DC_Director director;
    public DC_Game serverGame;

    [Space(10)]

    public DC_Player remotePlayer = null;

    [Space(10)]

    public DC_Avatar avatar;
    public GameObject avatarGO;

    public DC_Avatar_Spawn avatarSpawn;
    public GameObject avatarSpawnGO;
    
    [Space(10)]

	public DC_GameManager gameManager;
    public DC_Player_Pedestal playerPedestal;
    public DC_AvatarSync avatarSync;
    public DC_GridSelector gridSelector;
    public DC_SpawnSelector spawnSelector;


    void Start()
    {

    }

    public void SetRemotePlayer(DC_Player player = null)
    {
        remotePlayer = player;

        if(player)
        {
            player.homeRoom = this;

            player.localPlayer = localPlayer;
            
            player.gameGrid = gameGrid;
            player.serverGame = serverGame;
        }

        playerPedestal.TogglePlayerActive(!!player);
    }

    public void SetAvatar(DC_Avatar ava)
    {
        avatar = ava;

        if(ava)
        {
            avatarGO = ava.gameObject;

            avatarSync.SetAvatar(avatar);
        }
    }

    public void SetAvatarSpawn(DC_Avatar_Spawn avaS)
    {
        avatarSpawn = avaS;
        avatarSpawnGO = avaS.gameObject;
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }
}
