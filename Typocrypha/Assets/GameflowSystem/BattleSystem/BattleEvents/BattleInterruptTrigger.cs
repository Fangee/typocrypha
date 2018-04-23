using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleInterruptTrigger : BattleEventTrigger {

    //Dialogue object to play on trigger
    public GameObject interruptScene;
    //Returns true to signal that Battlemanager should pause
    public override bool onTrigger(BattleField state)
    {
        Debug.Log("Interrupt triggered: " + interruptScene.name);
        DialogueManager.main.enabled = true;
        DialogueManager.main.startInterrupt(interruptScene);
        triggered = true;
        return true;
    }
}
