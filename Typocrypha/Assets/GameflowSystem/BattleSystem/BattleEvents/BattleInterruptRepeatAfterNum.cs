using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleInterruptRepeatAfterNum : BattleInterruptRepeated {
    public int repeatAfterX = 1;
    private int curr_triggers = 0;
    //Returns true to signal that Battlemanager should pause
    public override bool onTrigger(Battlefield state)
    {
        if (++curr_triggers < repeatAfterX)
            return false;
        if (interruptScene != null)
        {
            Debug.Log("Interrupt triggered: " + interruptScene.name);
            state.addSceneToQueue(interruptScene);
        }
        curr_triggers = 0;
        reset(state);
        return true;
    }
}
