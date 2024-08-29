using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TriggerData
{
    public string id;
    public bool isActivate;
    public string startNPCId;
    public TriggerData(string id, bool isActive)
    {
        this.id = id;
        this.isActivate = isActive;
    }
}
