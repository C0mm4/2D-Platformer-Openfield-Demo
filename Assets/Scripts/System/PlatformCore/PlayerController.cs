using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static GameManager;
using UnityEngine.TextCore.Text;

public class PlayerController : KinematicObject
{
    public bool controlEnabled;

    public List<InteractionTrigger> triggers;
    public int triggerIndex;

    [SerializeField]
    public Charactor charactor;

    public void Awake()
    {

        triggers = new();
        gravityModifier = 1f;

        sawDir = new Vector2(1, 0);

    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        KeyInput();
    }

    public void Update()
    {
        if (controlEnabled)
        {
            if (Mathf.Abs(velocity.x) <= 0.05)
            {
                isMove = false;
            }
            else
            {
                isMove = true;
            }

            if (GetUIState() == UIState.InPlay)
            {


                // Interaction UI Generate
                if (triggers.Count > 0)
                {
                    GenInteractionUI();
                }
                else
                {
                    DeleteInteractionUI();
                }
            }

        }
    }


    public void KeyInput()
    {
        // is not Force Moving, can control player charactor
        if (!isForceMoving)
        {
            if (GetUIState() == UIState.InPlay)
            {
                if (controlEnabled)
                {
                    // Move and Skill Key
                    if (canMove)
                    {
                        MoveKey();
                    }
                    else
                    {
                        velocity = new Vector2(0, velocity.y);
                    }
                    
                    // Interaction Key
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        if (triggers.Count > 0)
                        {
                            triggers[triggerIndex].Interaction();

                        }
                        DeleteInteractionUI();
                    }
                }
                else
                {
                    // When Control enabled is false, add your Action

                }

            }
        }
        // is Force Moving, can't control player charactor
        else
        {
            Vector2 dir = new Vector2();
            if (targetMovePos.x > transform.position.x)
            {
                dir.x = 1;
            }
            else
            {
                dir.x = -1;
            }
            velocity = new Vector2(dir.x, velocity.y);
        }
    }

    public void MoveKey()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (canMove)
            {
                velocity = new Vector2(-charactor.charaData.activeMaxSpeed, velocity.y);
                sawDir = new Vector2(-1, sawDir.y);
            }

        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            if (canMove)
            {
                velocity = new Vector2(charactor.charaData.activeMaxSpeed, velocity.y);
                sawDir = new Vector2(1, sawDir.y);
            }

        }
        else
        {
            velocity = new Vector2(0f, velocity.y);
        }

        if (Input.GetKeyDown(KeyCode.S) && isGrounded && canMove)
        {
            charactor.Jump();
            isGrounded = false;
        }

    }


    public void AddInterractionTrigger(InteractionTrigger trigger)
    {
        if (!triggers.Contains(trigger))
        {
            triggers.Add(trigger);
        }
    }

    public void RemoveInteractionTrigger(InteractionTrigger trigger)
    {
        if (triggers.Contains(trigger))
        {
            triggers.Remove(trigger);
            if (triggerIndex >= triggers.Count)
            {
                triggerIndex = triggers.Count - 1;
            }
            if (triggerIndex < 0)
                triggerIndex = 0;
        }
    }

    public void GenInteractionUI()
    {
        GameManager.UIManager.GenerateInteractionUI();
    }

    public void DeleteInteractionUI()
    {
        GameManager.UIManager.DeleteInteractionUI();
    }


}
