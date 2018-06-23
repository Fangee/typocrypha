using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleInterruptRepeated : BattleInterruptTrigger {
    //Returns true to signal that Battlemanager should pause
    public override bool onTrigger(BattleField state)
    {
        if (interruptScene != null)
        {
            Debug.Log("Interrupt triggered: " + interruptScene.name);
            state.addSceneToQueue(interruptScene);
        }
        reset(state);
        return true;
    }
    public void reset(BattleField state) { }
}
