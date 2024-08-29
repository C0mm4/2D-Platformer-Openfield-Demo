using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class CutSceneTrigger : Trigger
{
    public override async Task Action()
    {
        await StartCutScene();
        await Task.Yield();
    }

    public override bool AdditionalCondition()
    {
        return true;
    }

    public async Task StartCutScene()
    {
        PlayerController player = GameManager.player.GetComponent<PlayerController>();
        GameManager.ChangeUIState(GameManager.UIState.CutScene);
        player.canMove = false;

        await ScriptPlay();
        

        GameManager.ChangeUIState(GameManager.UIState.InPlay);
        player.canMove = true;

    }

    public async Task ScriptPlay()
    {

        GameManager.ChangeUIState(GameManager.UIState.CutScene);


        for (int i = 0; i < trigText.scripts.Count; i++)
        {
            if (!trigText.scripts[i].junctionID.Equals(""))
            {
                if (GameManager.Progress.activateJunctions.FindIndex(item => item.Equals(trigText.scripts[i].junctionID)) == -1)
                {
                    continue;
                }
            }
            string npcId = trigText.scripts[i].npcId;
            if (npcId.Equals("99000000"))
            {
                await Func.Action(trigText.scripts[i].script);
            }
            else
            {
                NPC targetNPC = FindNPC(npcId);
                GameManager.CameraManager.player = targetNPC.transform;
                if (trigText.scripts[i].isAwait)
                {
#pragma warning disable CS4014
                    NPCSay(trigText.scripts[i], targetNPC);
#pragma warning restore CS4014
                    await Task.Delay(TimeSpan.FromMilliseconds(trigText.scripts[i].delayT));
                }
                else
                {
                    await NPCSay(trigText.scripts[i], targetNPC);
                }
            }
        }


        GameManager.CameraManager.player = GameManager.player.transform;
    }

    public async Task NPCSay(Script script, NPC targetNPC)
    {
        string applyScript = Func.ChangeStringToValue(script.script);
        await targetNPC.Say(applyScript);
    }
}
