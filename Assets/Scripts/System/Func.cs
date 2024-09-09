using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

public static class Func
{
    // Set gameobject Rect Transform
    public static void SetRectTransform(GameObject go, Vector3 pos = default)
    {
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.transform.SetParent(GameManager.UIManager.canvas.transform, false);
        rt.transform.localScale = Vector3.one;
        rt.transform.localPosition = pos;
    }
    
    // Script Action Function
    public static async Task Action(string action, bool isScene = true)
    {
        action = action.TrimStart('#');
        action = action.Trim(' ');
        string[] actions = action.Split('.');

        string targetNPCID;

        for (int i = 0; i < actions.Length; i++)
        {
            switch (actions[i])
            {
                case "Delay":
                    if (isScene)
                    {
                        float delayT = float.Parse(actions[++i]);
                        Debug.Log(delayT);
                        await Task.Delay(TimeSpan.FromMilliseconds(delayT));
                    }
                    break;
                case "Camera":
                    if (isScene)
                    {
                        targetNPCID = actions[++i];
                        if (targetNPCID.Equals("Player"))
                        {
                            GameManager.CameraManager.player = GameManager.player.transform;
                        }
                        else
                        {
                            NPC npc = FindNPC(targetNPCID, actions[++i]);
                            GameManager.CameraManager.player = npc.transform;
                        }
                    }
                    break;
                case "DoorActive":
                    if (isScene)
                    {
                        string door = actions[++i];
                        GameManager.Stage.DoorActivate(door);
                    }
                    break;
                case "DoorClose":
                    if (isScene)
                    {
                        string door = actions[++i];
                        GameManager.Stage.DoorDeActivate(door);
                    }
                    break;
                case "JunctionActivate":
                    GameManager.Progress.activateJunctions.Add(actions[++i]);
                    break;
            }
        }
    }

    // Find NPC
    public static NPC FindNPC(string id, string spawnP = null)
    {
        List<NPC> npcs = GameManager.Stage.currentMap.GetComponentsInChildren<NPC>().ToList();
        NPC ret = npcs.Find(item => item.npcId.Equals(id));


        return ret;
    }
}
