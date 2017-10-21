using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_NewGame_LightSphere : SubToolBase 
{
    private Vector3 oPos;
    private Quaternion oRot;
    private Transform oPar;

    public bool installed = false;

	public void Install(Vector3 newPos)
    {
        if(installed)
            return;

        oPos = originalPos;
        oRot = originalRot;
        oPar = originalPar;

        SetOrigin(newPos, oRot, oPar);

        installed = true;
    }

    public void UnInstall()
    {
        if(!installed)
            return;

        installed = false;
        SetOrigin(oPos, oRot, oPar);
    }
}
