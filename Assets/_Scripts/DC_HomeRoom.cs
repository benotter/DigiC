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

        (gameManager as DC_HR_Equipment_Base).OnJoinGame();
        (playerPedestal as DC_HR_Equipment_Base).OnJoinGame();
        (avatarSync as DC_HR_Equipment_Base).OnJoinGame();
        (gridSelector as DC_HR_Equipment_Base).OnJoinGame();
        (spawnSelector as DC_HR_Equipment_Base).OnJoinGame();
    }

    public void SetAvatar(DC_Avatar ava = null)
    {
        avatar = ava;
        avatarSync.SetAvatar(ava);

        if(ava)
            avatarGO = ava.gameObject;
        else
            avatarGO = null;
    }

    public void SetAvatarSpawn(DC_Avatar_Spawn avaS = null)
    {
        avatarSpawn = avaS;
        spawnSelector.SetAvatarSpawn(avaS);

        if(avaS)
            avatarSpawnGO = avaS.gameObject;
        else
            avatarSpawnGO = null;        
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public void GameStart()
    {

    }

    public void GameEnd()
    {

    }

    public void RoundStart()
    {

    }

    public void RoundEnd()
    {
        
    }
}
