using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_HomeRoom : MonoBehaviour 
{
    public DC_LocalPlayer localPlayer;

    [Space(10)]

    public DC_Player remotePlayer = null;

    [Space(10)]

    public Camera floorCam;
    
    [Space(10)]

	public DC_GameManager gameManager;
    public DC_Player_Pedestal playerPedestal;
    public DC_AvatarSync avatarSync;
    public DC_GridSelector gridSelector;

    void Start()
    {
        floorCam.enabled = false;
    }

    public void SetRemotePlayer(DC_Player player = null)
    {
        remotePlayer = player;

        if(player)
        {
            remotePlayer.transform.position = transform.position;
            transform.parent = remotePlayer.transform;
        }

        playerPedestal.TogglePlayerActive(!!player);
    }
}
