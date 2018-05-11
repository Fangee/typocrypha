﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleInterruptTrigger : BattleEventTrigger {

    //Dialogue object to play on trigger
    public GameObject interruptScene;
    //Returns true to signal that Battlemanager should pause
    public override bool onTrigger(BattleField state)
    {
        Debug.Log("Interrupt triggered: " + interruptScene.name);
        state.addSceneToQueue(interruptScene);
        triggered = true;
        return true;
    }
}
