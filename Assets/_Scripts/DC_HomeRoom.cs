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
    public bool gameJoined = false;


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

    public void RegisterGame(DC_Game game = null, DC_GameGrid grid = null) 
    {
        serverGame = game;
        gameGrid = grid;
    }

    public void SetRemotePlayer(DC_Player player = null)
    {
        remotePlayer = player;
        gameJoined = !!player;

        if(player)
        {
            player.localPlayer = localPlayer;

            player.homeRoom = this;
            player.serverGame = serverGame;    
            player.gameGrid = gameGrid;

            JoinGame();
        }
        else 
            LeaveGame();
    }

    public void SetAvatar(DC_Avatar ava = null)
    {
        avatar = ava;
        avatarGO = ava ? ava.gameObject : null;

        if(ava)
            GainAvatar();
        else
            LoseAvatar();

    }

    public void SetAvatarSpawn(DC_Avatar_Spawn avaS = null)
    {
        avatarSpawn = avaS;
        spawnSelector.SetAvatarSpawn(avaS);
        avatarSpawnGO = avaS ? avaS.gameObject : null;

        if(avaS)
            GainAvatarSpawn();
        else
            LoseAvatarSpawn();
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    DC_HR_Equipment_Base[] equipmentList;
    public DC_HR_Equipment_Base[] GetEquipmentList()
    {
        if(equipmentList == null)
            equipmentList = new DC_HR_Equipment_Base[] 
            {
                (gameManager as DC_HR_Equipment_Base),
                (playerPedestal as DC_HR_Equipment_Base),
                (avatarSync as DC_HR_Equipment_Base),
                (gridSelector as DC_HR_Equipment_Base),
                (spawnSelector as DC_HR_Equipment_Base),
            };

        return equipmentList;
    }

    public void JoinGame() 
    {
        foreach(DC_HR_Equipment_Base equip in GetEquipmentList())
            equip.OnJoinGame(remotePlayer);
    }

    public void LeaveGame() 
    {
        foreach(DC_HR_Equipment_Base equip in GetEquipmentList())
            equip.OnLeaveGame();
    }

    public void RoundStart()
    {
        foreach(DC_HR_Equipment_Base equip in GetEquipmentList())
            equip.OnRoundStart();
    }

    public void RoundEnd()
    {
        foreach(DC_HR_Equipment_Base equip in GetEquipmentList())
            equip.OnRoundEnd();
    }

    public void GainAvatarSpawn() 
    {
        foreach(DC_HR_Equipment_Base equip in GetEquipmentList())
            equip.OnGainAvatarSpawn(avatarSpawn);
    }
    public void LoseAvatarSpawn() 
    {
        foreach(DC_HR_Equipment_Base equip in GetEquipmentList())
            equip.OnLoseAvatarSpawn();
    }

    public void GainAvatar() 
    {
        foreach(DC_HR_Equipment_Base equip in GetEquipmentList())
            equip.OnGainAvatar(avatar);
    }

    public void LoseAvatar()
    {
        foreach(DC_HR_Equipment_Base equip in GetEquipmentList())
            equip.OnLoseAvatar();
    }
}
