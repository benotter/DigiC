using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_Player 
{
    public DC_Game serverGame;

    [Command]
    public void CmdRequestAvatarSpawn()
    {
        serverGame.RequestAvatarSpawn(this);
    }

    [Command]
    public void CmdRequestAvatar()
    {
        serverGame.RequestAvatar(this);
    }

    [Command]
    public void CmdSetGridPosition(int x, int y)
    {
        if(!gameGrid.CheckPosition(x, y))
            gameGrid.SetPosition(this.gameObject, x, y);
    }

    
}
