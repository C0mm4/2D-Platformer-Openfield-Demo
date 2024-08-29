using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using static GameManager;

public class NPC : InteractionTrigger
{

    public bool isSaying;

    public int sayingIndex;

    [SerializeField]
    public float sayingDelay;
    public float nextSayDelay;
    public GameObject letterBox;

    public Animator animator;
    public Animator glasses;

    public string npcId;

    public List<string> scripts;
    public int scriptIndex;
    [SerializeField]
    public List<NPCSetScript> SaveScripts;

    public Trigger sayTrigger;

    public GameObject triggerEffect;

    public int sayN = 0;

    public override void Awake()
    {
        sayingIndex = 0;
        scriptIndex = 0;
        isSaying = false;
        text = "대화한다";
        SaveScripts = new();
    }

    public override void FixedUpdate()
    {
        if (isForceMoving)
        {
            {
                var dir = targetMovePos - transform.position;
                if (!isLanding)
                {
                    if (dir.x < 0)
                    {
                        velocity = new Vector2(-1, velocity.y);
                        isMove = true;
                    }
                    else
                    {
                        velocity = new Vector2(1, velocity.y);
                        isMove = true;
                    }
                }
            }
        }
        base.FixedUpdate();
    }

    public override void Update()
    {
        base.Update();
        sayTrigger = CheckTriggerStart();
        if (GetUIState() == UIState.InPlay)
        {
            if (sayTrigger != null)
            {
                if (triggerEffect == null)
                {
                    triggerEffect = InstantiateAsync("TriggerEffect");
                    triggerEffect.transform.position = transform.position + new Vector3(0, 2.5f);
                }
            }
            else
            {
                if (triggerEffect != null)
                {
                    Resource.Destroy(triggerEffect);
                    triggerEffect = null;
                }
            }

            if (sayTrigger != null || scripts.Count != 0)
            {
                detectDistance = 2f;
            }
            else
            {
                detectDistance = 0f;
            }

        }
        else
        {
            if (triggerEffect != null)
            {
                Resource.Destroy(triggerEffect);
                triggerEffect = null;
            }
        }
    }



    public async Task Say()
    {

        int currentSayN = ++sayN;

        if (letterBox != null)
        {
            Resource.Destroy(letterBox);
        }

        isSaying = true;
        sayingIndex = 0;

        var awaitObj = GameManager.InstantiateAsync("LetterBox");
        letterBox = awaitObj;
        letterBox.GetComponent<LetterBox>().npc = this;
        letterBox.GetComponent<LetterBox>().SetPosition();
        letterBox.transform.SetParent(GameManager.UIManager.canvas.transform, false);

        float sayT = 0f;

        if (scripts[scriptIndex][sayingIndex].Equals('#'))
        {
            await Func.Action(scripts[scriptIndex]);
            if(scriptIndex < scripts.Count - 1)
            {
                scriptIndex++;
            }
        }

        while (sayingIndex <= scripts[scriptIndex].Length - 1)
        {
            if (currentSayN != sayN)
            {
                return;
            }
            sayT += Time.deltaTime;
            if (sayT >= sayingDelay)
            {
                sayingIndex++;
                if (sayingIndex < scripts[scriptIndex].Length)
                {
                    if (scripts[scriptIndex][sayingIndex].Equals(" "))
                    {
                        sayingIndex++;
                    }
                }
                var cuttext = scripts[scriptIndex][..sayingIndex];

                letterBox.GetComponent<LetterBox>().SetText(cuttext);
                sayT = 0f;
            }
            if (!isSaying)
            {
                break;
            }
            await Task.Yield();
        }
        isSaying = false;

        if (sayingIndex == letterBox.GetComponentInChildren<TMP_Text>().text.Length)
        {
            await Task.Delay(TimeSpan.FromSeconds(1f));
        }

        Resource.Destroy(letterBox);

        await Task.Delay(TimeSpan.FromSeconds(nextSayDelay));


    }


    public async Task Say(string script)
    {
        int currentSayN = ++sayN;

        if (letterBox != null)
        {
            Resource.Destroy(letterBox);
        }

        isSaying = true;
        sayingIndex = 0;

        var awaitObj = GameManager.InstantiateAsync("LetterBox");
        letterBox = awaitObj;
        letterBox.GetComponent<LetterBox>().npc = this;
        letterBox.GetComponent<LetterBox>().SetPosition();
        letterBox.transform.SetParent(GameManager.UIManager.canvas.transform, false);

        float sayStartT = 0f;
        float sayT = 0f;

        while (sayingIndex <= script.Length - 1)
        {
            if (currentSayN != sayN)
            {
                return;
            }
            sayT += Time.deltaTime;
            sayStartT += Time.deltaTime;
            if (sayT >= sayingDelay)
            {
                // if Script Action Tokken get
                // Script Action Play
                if (script[sayingIndex].Equals('#'))
                {
                    int actionStartIndex = sayingIndex;
                    string action;
                    // Find Action Escape Tokken
                    while (!script[sayingIndex].Equals(' '))
                    {
                        sayingIndex++;
                    }
                    // Find Action Script
                    action = script[actionStartIndex..sayingIndex++];

                    // Play Action Script

                    await Func.Action(action);

                    // Remove Action Script 
                    script = script.Remove(actionStartIndex, sayingIndex - actionStartIndex);

                    // Return Index to action start index;
                    sayingIndex = actionStartIndex;
                }
                // if Script Size Select Tokken get
                // Script Play
                if (script[sayingIndex].Equals('<'))
                {
                    while (!script[sayingIndex].Equals('>'))
                    {
                        sayingIndex++;
                    }

                    var cuttext = script[..++sayingIndex];
                    letterBox.GetComponent<LetterBox>().SetText(cuttext);
                    sayT = 0f;
                }
                else
                {
                    sayingIndex++;
                    if (sayingIndex < script.Length)
                    {
                        if (script[sayingIndex].Equals(" "))
                        {
                            sayingIndex++;
                        }
                    }
                    var cuttext = script[..sayingIndex];

                    letterBox.GetComponent<LetterBox>().SetText(cuttext);
                    sayT = 0f;
                }

            }
            if (!isSaying)
            {
                break;
            }
            await Task.Yield();
        }
        isSaying = false;

        if (sayingIndex == letterBox.GetComponentInChildren<TMP_Text>().text.Length)
        {
            await Task.Delay(TimeSpan.FromSeconds(1f));
        }

        GameManager.Destroy(letterBox);

        await Task.Delay(TimeSpan.FromSeconds(nextSayDelay));


    }
    public async Task Action(string action)
    {
        switch (action.Split('.')[0])
        {
            case "#Delay":
                Debug.Log(action.Split('.')[1]);
                await Task.Delay(TimeSpan.FromMilliseconds(float.Parse(action.Split('.')[1].Trim())));
                break;
            case "Camera":
                break;
            case "Spawn":
                break;

        }
    }

    public override async void Interaction()
    {
        base.Interaction();
        Trigger trig = CheckTriggerStart();
        if (trig != null)
        {
            await trig.TriggerActive();
        }
        else
        {
            ChangeUIState(UIState.CutScene);
            await Say();
            if (scriptIndex < scripts.Count - 1)
            {
                scriptIndex++;
            }
            ChangeUIState(UIState.InPlay);

        }
    }

    public void AddScripts(TrigScript trigScript)
    {
        NPCSetScript newScriptTrig = new NPCSetScript();
        newScriptTrig.triggerID = trigScript.trigId;
        newScriptTrig.NPCID = npcId;
        newScriptTrig.scripts = new();
        foreach (NPCScript script in trigScript.scripts)
        {
            if (script.npcId.Equals(npcId))
            {
                newScriptTrig.scripts.Add(script.script);
            }
        }
        if (newScriptTrig.scripts.Count > 0)
        {
            SaveScripts.Add(newScriptTrig);
        }
    }

    public void SetScripts()
    {
        foreach (NPCSetScript scriptData in SaveScripts)
        {
            string targetTrig = scriptData.triggerID;
            if (targetTrig.Equals(""))
            {
                scripts = scriptData.scripts;
                scriptIndex = 0;
            }
            else if (Progress.activeTrigs.ContainsKey(targetTrig))
            {
                scripts = scriptData.scripts;
                scriptIndex = 0;
            }
        }
    }

    private Trigger CheckTriggerStart()
    {
        foreach (Trigger trig in Stage.currentMapTrigger)
        {
            if (!trig.data.isActivate && trig.CheckNodesActive())
            {
                if (trig.data.startNPCId.Equals(npcId))
                {
                    return trig;
                }
            }
        }

        return null;
    }
}
