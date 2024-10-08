using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnner
{

    
    public NPC NPCSpawn(string id, Vector3 pos)
    {
        GameObject ret = GameManager.InstantiateAsync(id, pos);
        ret.GetComponent<NPC>().npcId = id;
        ret.transform.SetParent(GameManager.Stage.currentMap.transform);
        GameManager.Stage.NPCScriptSet(ret.GetComponent<NPC>());
        GameManager.Stage.currentNPCs.Add(ret.GetComponent<NPC>());
        return ret.GetComponent<NPC>();
    }
}
