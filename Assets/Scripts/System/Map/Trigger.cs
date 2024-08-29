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

    public int SelectIndex;

    public TrigText trigText;

    public List<string> nextTriggerId;
    public List<Trigger> nextTrigger;

    public List<GameObject> conditionObjs;
    public List<GameObject> spawnObjs;


    [Serializable]

    class AdditionalCondi
    {
        [SerializeField]
        enum condiType
        {
            None, // Add More Conditions
        };

        [SerializeField]
        condiType condition;

        [SerializeField]
        float HPRatio;

        public Trigger originTrigger;

        public bool CheckCondi()
        {
            switch (condition)
            {
                case condiType.None: return true;
                // Add More Condition Return
                default: return true;
            }
        }

    }
    [SerializeField]
    AdditionalCondi condi;

    public void Awake()
    {
        triggerBox = GetComponent<Collider2D>();
        triggerBox.isTrigger = true;
        data.isActivate = false;
        conditionObjs = new();
        spawnObjs = new();
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
        if (data.startNPCId.Equals(""))
        {
            if (collision.gameObject == GameManager.player)
            {
                if (GameManager.GetUIState() == GameManager.UIState.InPlay)
                {
                    if (!data.isActivate)
                    {
                        if (nodeIds.Count == 0)
                        {
                            if (condi.CheckCondi())
                            {
                                await TriggerActive();
                            }
                        }
                        else
                        {
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
        await Action();

        await Task.Run(() =>
        {
            GameManager.Trigger.ActiveTrigger(data);
        });

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
