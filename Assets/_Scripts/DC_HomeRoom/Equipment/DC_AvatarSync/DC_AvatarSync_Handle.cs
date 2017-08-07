using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_AvatarSync_Handle : SubToolBase 
{
    public DC_AvatarSync_Handle twin;

    [Space(10)]
	public DC_AvatarSync avatarSync;

    [HideInInspector]
    public bool firstToStart = false;

    private bool doubleMenuDown = false;
    private float doubleMenuDownTime = 0f;

    void Start()
    {
        if(!twin.firstToStart)
            firstToStart = true;
    }

    void Update()
    {
        base.UpdateSnapping();
        
        if(!inUse)
            return;

        UpdateMenuStat();

        if(avatarSync.linked && dropable)
            dropable = false;
        else if(!dropable)
            dropable = true;
    }

    void UpdateMenuStat()
    {
        if(doubleMenuDown)
            doubleMenuDownTime += Time.deltaTime;

        if(!doubleMenuDown && (menuButton && twin.menuButton))
            doubleMenuDown = true;

        if(doubleMenuDown && !(menuButton && twin.menuButton))
        {
            doubleMenuDown = false;
            doubleMenuDownTime = 0f;
        }
    }
}
