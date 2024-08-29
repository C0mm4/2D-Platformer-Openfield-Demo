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
        text = "이동한다";
        if (isActivate)
        {
            detectDistance = 1f;
        }
    }

    public override void Update()
    {
        base.Update();
        if (isActivate)
        {
            detectDistance = 1f;
        }
        else
        {
            detectDistance = 0f;
        }
    }

    public override async void Interaction()
    {
        if (isActivate)
        {
            base.Interaction();
            await GameManager.Scene.MoveMap(connectRoomId, connectDoorId);
        }
    }
}
