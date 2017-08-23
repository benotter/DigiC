using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_Avatar
{
    public void UpdatePosition()
    {
        if(!inMove && (lastColFlags & CollisionFlags.Below) == 0)
            currentVelocity.y -= gravity * Time.deltaTime;

        var move = targetMove + currentVelocity * Time.deltaTime;

        if(Mathf.Abs(move.x) > terminalVelocity)
            move.x = move.x > 0 ? terminalVelocity : -terminalVelocity;
                    
        if(Mathf.Abs(move.y) > terminalVelocity)
            move.y = move.y > 0 ? terminalVelocity : -terminalVelocity;

        if(Mathf.Abs(move.z) > terminalVelocity)
            move.z = move.z > 0 ? terminalVelocity : -terminalVelocity;

        lastColFlags = coll.Move(move);

        if(lastColFlags == CollisionFlags.None)
            currentVelocity -= (drag * currentVelocity) * Time.deltaTime;
        else
            ResetVelocity();

        targetMove = Vector3.zero;
    }

    public void UpdateBody()
    {
        var mT = muzzle.transform;
        var cT = chest.transform;

        var hmdP = localPlayer.hmdPos;
           
        mT.localPosition = new Vector3(0f, hmdP.y, 0f);
        mT.localRotation = localPlayer.hmdRot;

        cT.localPosition = mT.localPosition;
        
        bool right = true, 
             left = false;

        if(controllersFlipped)
        {
            right = !right;
            left = !left;
        }

        UpdatePaw(rightPaw.transform, right);
        UpdatePaw(leftPaw.transform, left);

        var acP = avatarCam.transform.parent;
        var aP = avatarCam.transform.localPosition;

        var newAP = -aP;
        newAP.y = 0;

        acP.localPosition = newAP;
    }

    public void UpdatePaw(Transform p, bool rightH = true)
    {
        var hmdP = localPlayer.hmdPos;

        Vector3 pos = rightH ? localPlayer.rightPos : localPlayer.leftPos;
        Quaternion rot = rightH ? localPlayer.rightRot : localPlayer.leftRot;

        p.localPosition = new Vector3(pos.x - hmdP.x, pos.y, pos.z - hmdP.z);
        p.localRotation = rot;
    }

    void UpdateCharacterController()
    {
        var hmdP = localPlayer.hmdPos;
        if(lastHmdPos != hmdP)
        {
            var mP = muzzle.transform.localPosition;

            coll.height = mP.y;
            coll.center = new Vector3(0f, mP.y / 2f, 0f);

            var move = hmdP - lastHmdPos;
            move.y = 0f;

            lastHmdPos = hmdP;

            MoveTo(move);
        }    
    }
}