using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionTrigger : KinematicObject
{
    public PlayerController player;
    public float detectDistance;
    public string text;


    public virtual void Awake()
    {
        var layerIndex = LayerMask.NameToLayer("InteractionObject");
        gameObject.layer = layerIndex;
    }

    public virtual void Update()
    {
        if (player == null)
        {
            FindPlayer();
        }
        else if (PlayerDistance() <= detectDistance)
        {
            player.AddInterractionTrigger(this);
        }
        else
        {
            player.RemoveInteractionTrigger(this);
        }
    }

    public float PlayerDistance()
    {
        float distance = Vector2.Distance(transform.position, player.transform.position);

        return distance;
    }

    public void FindPlayer()
    {
        if (GameManager.player != null)
        {
            player = GameManager.player.GetComponent<PlayerController>();
        }
    }

    public virtual void Interaction()
    {

    }
}
