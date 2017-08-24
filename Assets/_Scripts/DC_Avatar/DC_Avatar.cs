using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class DC_Avatar : NetworkBehaviour 
{
    // Client-Side Variables
    public DC_LocalPlayer localPlayer;

    [Space(10)]

    public int rightStartToolIndex = -1;
    public int leftStartToolIndex = -1;

    public GameObject[] toolList = new GameObject[0];
    
    [Space(10)]

    public Camera avatarCam;

    [Space(10)]

    public GameObject muzzle;
    public GameObject chest;
    public GameObject rightPaw;
    public GameObject leftPaw;

    [Space(10)]

    public DC_Avatar_Tool rightTool;
    public DC_Avatar_Tool leftTool;

    [Space(10)]

    // Server-Side Variables

    [SyncVar] public float gravity = 1f;
    [SyncVar] public float drag = 1f;
    [SyncVar] public float terminalVelocity = 10f;

    [Space(10)]

    [SyncVar] public GameObject playerO;

    [Space(10)]

    [SyncVar] public GameObject rightToolO;
    [SyncVar] public GameObject leftToolO;
    
    [Space(10)]

    [SyncVar] public bool linked = false;

    [HideInInspector] public bool inMove = false;


    // Public Client-Side Variables

    [HideInInspector] public bool controllersFlipped = false;
    
    // Private Client-Side Variables

    private CharacterController coll;

    private bool wasLinked = false;
    
    private Vector3 targetMove = Vector3.zero;
    
    private Vector3 lastHmdPos = Vector3.zero;
    private Vector3 lastPos = Vector3.zero;

    public Vector3 moveVelocity = Vector3.zero;
    public Vector3 currentVelocity = Vector3.zero;

    private CollisionFlags lastColFlags;


    static private int avatarLocalLayer = 0;
    static private int avatarPawsLayer;
    static private int avatarChestLayer;
    static private int avatarRemoteLayer;
    static private int avatarServerLayer;


    void Start()
    {
        NetworkTransform netT = GetComponent<NetworkTransform>();

        foreach(NetworkTransformChild childT in GetComponents<NetworkTransformChild>())
        {
            childT.sendInterval = netT.sendInterval;
            childT.interpolateMovement = netT.interpolateMovement;
            childT.interpolateRotation = netT.interpolateRotation;
            childT.rotationSyncCompression = netT.rotationSyncCompression;
        }

        if(avatarLocalLayer == 0)
        {
            avatarLocalLayer = LayerMask.NameToLayer("Avatar_Local");

            avatarPawsLayer = LayerMask.NameToLayer("Avatar_Paws");
            avatarChestLayer = LayerMask.NameToLayer("Avatar_Chest");

            avatarRemoteLayer = LayerMask.NameToLayer("Avatar_Remote");
            avatarServerLayer = LayerMask.NameToLayer("Avatar_Server");
        }
    }

    public override void OnStartServer()
    {
        // coll = GetComponent<CharacterController>();

        muzzle.transform.GetChild(0).gameObject.layer = avatarServerLayer;
        chest.transform.GetChild(0).gameObject.layer = avatarServerLayer;

        rightPaw.transform.GetChild(0).gameObject.layer = avatarServerLayer;
        leftPaw.transform.GetChild(0).gameObject.layer = avatarServerLayer;
    }

    public override void OnStartClient()
    {

    }

    public override void OnStartAuthority()
    {
        avatarCam.enabled = true;

        var layer = LayerMask.NameToLayer("Avatar_Local");

        muzzle.transform.GetChild(0).gameObject.layer = layer;
        chest.transform.GetChild(0).gameObject.layer = layer;

        coll = GetComponent<CharacterController>();

        if(rightStartToolIndex > -1)
            CmdRequestTool(rightStartToolIndex, 0);

        if(leftStartToolIndex > -1)
            CmdRequestTool(leftStartToolIndex, 1);
    }

    public override void OnStopAuthority()
    {
        avatarCam.enabled = false;
    }

    public override void OnNetworkDestroy() 
    {

    }

    [Server] public void ServerBoltStrike(DC_Bolt bolt, BodyParts bodyPos)
    {
        switch(bodyPos)
        {
            case BodyParts.Chest:
            break;

            case BodyParts.Muzzle:
            break;

            case BodyParts.RightPaw:
            break;

            case BodyParts.LeftPaw:
            break;

            case BodyParts.ChestChord:
            break;

            case BodyParts.MuzzleChord:
            break;
        }
    }

    [Client] public void ClientBoltStrike(DC_Bolt bolt, BodyParts bodyPos)
    {
        switch(bodyPos)
        {
            case BodyParts.Chest:
            break;

            case BodyParts.Muzzle:
            break;

            case BodyParts.RightPaw:
            break;

            case BodyParts.LeftPaw:
            break;

            case BodyParts.ChestChord:
            break;

            case BodyParts.MuzzleChord:
            break;
        }
    }

    public void SyncAvatarTools(DC_AvatarSync_Handle rightHandle, DC_AvatarSync_Handle leftHandle)
    {
        if(rightTool)
            rightTool.UpdateState(rightHandle);

        if(leftTool)
            leftTool.UpdateState(leftHandle);
    }

    public void StartMove()
    {
        inMove = true;
    }

    public void StopMove()
    {
        inMove = false;
    }

    public void MoveTo(Vector3 worldPoint)
    {
        targetMove += worldPoint;
    }

    public void MoveTo(Vector3 worldPoint, Vector3 moveTarget)
    {
        MoveTo(worldPoint);
    }

    public void ResetVelocity()
    {
        currentVelocity = Vector3.zero;
    }

    public void SetLocalPlayer(DC_LocalPlayer p = null)
    {
        localPlayer = p;
    }


    // Server-Side Commands    


    [Command] public void CmdStartLink()
    {
        linked = true;
        wasLinked = true;
    }
    
    [Command] public void CmdStopLink()
    {
        linked = false;
    }

    [Command] public void CmdRequestTool(int toolIndex, int hand)
    {
        if(toolIndex >= toolList.Length)
            return;

        GameObject toolB = toolList[toolIndex];

        GameObject toolO = Instantiate(toolB);
        
        if(NetworkServer.SpawnWithClientAuthority(toolO, playerO))
        {
            DC_Avatar_Tool tool = toolO.GetComponent<DC_Avatar_Tool>();

            tool.avatarO = gameObject;
            tool.playerO = playerO;

            if(hand == 0)
            {
                if(rightToolO)
                    Network.Destroy(rightToolO);

                rightToolO = toolO;
            }
            else
            {
                if(leftToolO)
                    Network.Destroy(leftToolO);

                leftToolO = toolO;
            }

            tool.RpcSetAvatar(gameObject);
            RpcSetAvatarTool(toolO, hand);
        }
    }


    // Client-Side Commands
    

    [ClientRpc] public void RpcBoltStrike(GameObject boltO, BodyParts part)
    {
        DC_Bolt bolt = boltO.GetComponent<DC_Bolt>();
        if(bolt)
            ClientBoltStrike(bolt, part);
    }

    [ClientRpc] public void RpcSetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    [ClientRpc] public void RpcSetAvatarTool(GameObject toolO, int hand)
    {
        DC_Avatar_Tool tool = toolO.GetComponent<DC_Avatar_Tool>();
        if(tool)
        {
            Transform h = hand == 0 ? rightPaw.transform : leftPaw.transform;

            toolO.transform.parent = h;
            toolO.transform.localEulerAngles = Vector3.zero;
            toolO.transform.localPosition = Vector3.zero;

            if(hand == 0)
                rightTool = tool;
            else
                leftTool = tool;
        }
    }

    [ClientRpc] public void RpcClearTool(int hand)
    {
        if(hand == 0)
            rightToolO = null;
        else
            leftToolO = null;
    }
}

public partial class DC_Avatar 
{
    public enum BodyParts 
    {
        None,
        Muzzle,
        Chest,
        RightPaw,
        LeftPaw,
        MuzzleChord,
        ChestChord
    }
}