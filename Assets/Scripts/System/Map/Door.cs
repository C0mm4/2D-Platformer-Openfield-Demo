using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : InteractionTrigger
{
    public Vector2 InDir;

    public string id;
    public string connectRoomId;
    public string connectDoorId;

    public bool isActivate;

    public override void Awake()
    {
        base.Awake();
        // is Door Activate, player Detect distance set
        if (isActivate)
        {
            detectDistance = 1f;
        }
    }

    public override void Update()
    {
        base.Update();
        // is Door Activate, player Detect distance set
        if (isActivate)
        {
            detectDistance = 1f;
        }
        // if Door is not activate, player Detect distance is 0
        else
        {
            detectDistance = 0f;
        }
    }

    public override async void Interaction()
    {
        // Door is Activate, Move Map
        if (isActivate)
        {
            base.Interaction();
            await GameManager.Scene.MoveMap(connectRoomId, connectDoorId);
        }
    }
}
