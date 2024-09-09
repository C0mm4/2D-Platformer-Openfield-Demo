using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerManager
{
    public void Init()
    {

    }
    
    public void ActiveTrigger(TriggerData trig)
    {
        // Add activated trigger data in gameprogress
        GameManager.Progress.activeTrigs[trig.id] = trig;

        // trigger data isActivate is true
        trig.isActivate = true;        
    }
}
