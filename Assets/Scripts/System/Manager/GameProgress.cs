using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
[Serializable]
public class GameProgress
{
    // current Map ID
    public string saveMapId;

    // open doors list
    public List<string> openDoors = new();

    // Activated triggers list
    public Dictionary<string, TriggerData> activeTrigs;

    // Activated junctions list
    public List<string> activateJunctions;
}
