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
        // Find player and set player cannot move, change uistate by cutscene
        PlayerController player = GameManager.player.GetComponent<PlayerController>();
        GameManager.ChangeUIState(GameManager.UIState.CutScene);
        player.canMove = false;

        // Interaction UI Destroy
        GameManager.UIManager.DeleteInteractionUI();

        // Trigger Script Play
        await ScriptPlay();
        
        // Reset uistate and player can move
        GameManager.ChangeUIState(GameManager.UIState.InPlay);
        player.canMove = true;

    }

    public async Task ScriptPlay()
    {
        // Loop Trigger Text Scripts
        for (int i = 0; i < trigText.scripts.Count; i++)
        {
            // if script has junctions, checking exists junction id in gameProgress
            if (!trigText.scripts[i].junctionID.Equals(""))
            {
                // if junction is not activated, ignore script
                if (GameManager.Progress.activateJunctions.FindIndex(item => item.Equals(trigText.scripts[i].junctionID)) == -1)
                {
                    continue;
                }
            }

            string npcId = trigText.scripts[i].npcId;
            // if NPC Id is Action ID -99000000  Play Action Script
            if (npcId.Equals("99000000"))
            {
                await Func.Action(trigText.scripts[i].script);
            }
            else
            {
                // Find target NPC
                NPC targetNPC = FindNPC(npcId);

                // camera target is target NPC
                GameManager.CameraManager.player = targetNPC.transform;

                // if NPC Script is not await, NPC say without await and delay script delay time
                if (!trigText.scripts[i].isAwait)
                {
#pragma warning disable CS4014
                    NPCSay(trigText.scripts[i], targetNPC);
#pragma warning restore CS4014
                    await Task.Delay(TimeSpan.FromMilliseconds(trigText.scripts[i].delayT));
                }
                else
                {
                    // Wait end NPC saying
                    await NPCSay(trigText.scripts[i], targetNPC);
                }
            }
        }

        // camera target is player object
        GameManager.CameraManager.player = GameManager.player.transform;
    }

    public async Task NPCSay(Script script, NPC targetNPC)
    {
        // Add your NPC Say Actions

        // wait NPC Say
        await targetNPC.Say(script.script);
    }
}
