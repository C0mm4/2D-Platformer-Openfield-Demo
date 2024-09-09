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
        // Find Trigger Objects
        Trigger[] triggers = GetComponentsInChildren<Trigger>();

        // Add your Trigger Data Set


    }

}
