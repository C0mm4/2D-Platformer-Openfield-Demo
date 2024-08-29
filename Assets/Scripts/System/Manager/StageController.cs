using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageController
{
    public Map currentMap;

    public MapTrigText currentMapTrigTexts;
    public MapScript currentMapNPCTexts;

    public List<Trigger> currentMapTrigger;

    public List<NPC> currentNPCs;

    public bool isTapInput = false;
    public int changeSlot = 0;
    public float changeInputT;


    public void LoadMap(string mapId)
    {

        GameObject go = GameManager.InstantiateAsync("Map_"+mapId);
        currentMap = go.GetComponent<Map>();
        currentMap.CreateHandler();
        GameManager.Progress.saveMapId = mapId;
        GameManager.CameraManager.background = go.GetComponent<Map>().bound;

        GameManager.Stage.currentMap = go.GetComponent<Map>();

        // Set Map Trigger Text Datas
        currentMapTrigTexts = GameManager.Script.getMapTrigTextData(mapId);
        Trigger[] triggers = go.GetComponentsInChildren<Trigger>();
        currentMapTrigger = new();
        foreach (Trigger trig in triggers)
        {
            if (!trig.data.id.Equals("SpawnTrigger"))
            {
                TrigText trigText = currentMapTrigTexts.trigTexts.Find(item => item.trigId.Equals(trig.data.id));
                trig.SetTriggerTextData(trigText);
                if (!trigText.scripts[0].startNPCId.Equals(""))
                {
                    trig.data.startNPCId = trigText.scripts[0].startNPCId;
                }
            }
            currentMapTrigger.Add(trig);
        }


        // Set Map NPC Text Datas (add later)
        currentMapNPCTexts = GameManager.Script.getMapScriptsData(mapId);
        currentNPCs = new();
        currentNPCs = currentMap.NPCs;

        if (currentMapNPCTexts != null)
        {
            var scripts = currentMapNPCTexts.scripts;
            foreach (TrigScript script in scripts)
            {
                foreach (NPC npc in currentNPCs)
                {
                    npc.AddScripts(script);
                }
            }

        }

        // SetNPC Text in Load
        RefreshNPCScript();

        // Set Doors Activate Datas
        var doors = currentMap.GetComponentsInChildren<Door>().ToList();
        foreach (Door door in doors)
        {
            if (GameManager.Progress.openDoors.Contains(door.id))
            {
                door.isActivate = true;
            }
        }
    }

    public void NPCScriptSet(NPC npc)
    {
        if (currentMapNPCTexts != null)
        {
            foreach (TrigScript script in currentMapNPCTexts.scripts)
            {
                npc.AddScripts(script);
            }

            npc.SetScripts();
        }
    }

    public void RefreshNPCScript()
    {
        foreach (var npc in currentMap.NPCs)
        {
            npc.SetScripts();
        }
    }

    public void DoorActivate(string id)
    {
        GameManager.Progress.openDoors.Add(id);
        var doors = currentMap.GetComponentsInChildren<Door>().ToList();
        Door targetDoor = doors.Find(item => item.id == id);
        if (targetDoor != null)
        {
            targetDoor.isActivate = true;
        }
    }

    public void DoorDeActivate(string id)
    {
        if (GameManager.Progress.openDoors.Contains(id))
        {
            GameManager.Progress.openDoors.Remove(id);
        }

        var doors = currentMap.GetComponentsInChildren<Door>().ToList();
        Door targetDoor = doors.Find(item => item.id == id);
        if (targetDoor != null)
        {
            targetDoor.isActivate = false;
        }
    }
}
