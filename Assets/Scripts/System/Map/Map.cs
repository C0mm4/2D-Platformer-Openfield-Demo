using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Map : MonoBehaviour
{
    public SpriteRenderer bound;
    public List<NPC> NPCs;

    public void Awake()
    {
        SetTriggerDatas();
    }

    public void CreateHandler()
    {
        NPCs = GetComponentsInChildren<NPC>(true).ToList();
    }

    public void SetTriggerDatas()
    {
        Trigger[] triggers = GetComponentsInChildren<Trigger>();

        foreach (var trigger in triggers)
        {
            foreach (string nextId in trigger.nextTriggerId)
            {
                foreach (var trig2 in triggers)
                {
                    if (trig2.data.id.Equals(nextId))
                    {
                        trigger.nextTrigger.Add(trig2);
                    }
                }
            }
        }
    }

}
