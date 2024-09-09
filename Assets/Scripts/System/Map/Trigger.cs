using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public abstract class Trigger : MonoBehaviour
{
    public List<string> nodeIds;

    protected Collider2D triggerBox;

    public TriggerData data;

    public TrigText trigText;


    [Serializable]
    class AdditionalCondi   // Additional Condition Class
    {
        [SerializeField]
        enum condiType
        {
            None, Junction, // Add More Conditions
        };

        [SerializeField]
        condiType condition;
        [SerializeField]
        List<string> junctionNodes;

        [SerializeField]
        float HPRatio;

        public Trigger originTrigger;

        public bool CheckCondi()
        {
            switch (condition)
            {
                case condiType.None: return true;
                case condiType.Junction: return CheckJunction();
                // Add More Condition Return


                default: return true;
            }
        }
        // Type is Junction, Check junctions
        private bool CheckJunction()
        {
            foreach(string junction in junctionNodes)
            {
                if(GameManager.Progress.activateJunctions.FindIndex(item => item.Equals(junction)) == -1)
                {
                    return false;
                }
            }
            return true;
        }
    }
    [SerializeField]
    AdditionalCondi condi;

    public void Awake()
    {
        triggerBox = GetComponent<Collider2D>();
        triggerBox.isTrigger = true;
        data.isActivate = false;
        if (GameManager.Progress.activeTrigs.ContainsKey(data.id))
        {
            data.isActivate = true;
        }
        else
        {
            data.isActivate = false;
        }/*
        string id = GetType().Name;
        id = id.Replace("Trig", "");
        data.id = id;*/
        condi.originTrigger = this;
    }

    public async virtual void OnTriggerStay2D(Collider2D collision)
    {
        // if Trigger start NPC data isn't exists
        if (data.startNPCId.Equals(""))
        {
            // if Player Collision
            if (collision.gameObject == GameManager.player)
            {
                // if GameState is In Play
                if (GameManager.GetUIState() == GameManager.UIState.InPlay)
                {
                    // is not already activate this trigger
                    if (!data.isActivate)
                    {
                        // node trigger is not exists
                        if (nodeIds.Count == 0)
                        {
                            // Check this trigger condition
                            if (condi.CheckCondi())
                            {
                                await TriggerActive();
                            }
                        }
                        else
                        {
                            // node trigger is all activate, and check this trigger condition
                            if (CheckNodesActive())
                            {
                                await TriggerActive();
                            }
                        }
                    }

                }
            }
        }
    }


    public virtual async Task TriggerActive()
    {
        GameManager.Trigger.ActiveTrigger(data);
        await Action();

        // Refresh NPC Scripts end Triggers
        GameManager.Stage.RefreshNPCScript();
    }

    public bool CheckNodesActive()
    {
        foreach (string node in nodeIds)
        {
            if (!GameManager.Progress.activeTrigs.ContainsKey(node))
            {
                return false;
            }
        }
        return condi.CheckCondi();

    }

    public abstract bool AdditionalCondition();


    public void BeforeUpdate()
    { 
        if (GameManager.Progress.activeTrigs.ContainsKey(data.id))
        {
            data.isActivate = true;
        }
        else
        {
            data.isActivate = false;
        }
    }

    public void SetTriggerTextData(TrigText txts)
    {
        trigText = txts;

        if (GameManager.Progress.activeTrigs.ContainsKey(data.id))
        {
            data.isActivate = true;
        }
        else
        {
            data.isActivate = false;
        }

        // is this trigger is activate already
        if (data.isActivate)
        {
            // Add NPC Spawn Despawn Data for Map Load
            foreach (Script script in trigText.scripts)
            {
                if (script.npcId.Equals("99000000"))
                {
#pragma warning disable CS4014 // 이 호출을 대기하지 않으므로 호출이 완료되기 전에 현재 메서드가 계속 실행됩니다.
                    Func.Action(script.script);
#pragma warning restore CS4014 // 이 호출을 대기하지 않으므로 호출이 완료되기 전에 현재 메서드가 계속 실행됩니다.
                }
            }
        }
    }

    public abstract Task Action();

    public NPC FindNPC(string id, Transform trans)
    {
        /*NPC ret = npcList.Find(item => item.npcId.Equals(id));*/
        List<NPC> npcs = GameManager.Stage.currentMap.NPCs;
        NPC ret = npcs.Find(item => item.npcId.Equals(id));
        if (ret == null)
        {
            ret = NPCSpawn(id, trans);
        }
        return ret;

    }

    public NPC FindNPC(string id)
    {
        List<NPC> npcs = GameManager.Stage.currentMap.NPCs;
        NPC ret = npcs.Find(item => item.npcId.Equals(id));

        return ret;
    }

    public NPC NPCSpawn(string id, Transform trans)
    {
        NPC ret = GameManager.Spawnner.NPCSpawn(id, trans.position).GetComponent<NPC>();
        return ret;
    }


}
