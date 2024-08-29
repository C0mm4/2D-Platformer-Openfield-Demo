using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
[Serializable]
public class GameProgress
{
    public string saveMapId;
    public List<string> openDoors = new();
    public Dictionary<string, TriggerData> activeTrigs;
    public List<string> activateJunctions;
}
