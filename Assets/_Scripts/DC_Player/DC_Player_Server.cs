using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_Player 
{
    public DC_Game serverGame;

    [Command]
    public void CmdRequestAvatar()
    {
        serverGame.RequestAvatar(this);
    }
}
