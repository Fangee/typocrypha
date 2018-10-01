using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BICheckHealth : BattleInterruptTrigger {
    public Battlefield.Position whoToCheck;
    public float healthRatio;

    public override bool checkTrigger(Battlefield state)
    {
        //      int index = (int)whoToCheck;
        //      ICaster toCheck = null;
        //      if (index < 3)
        //          toCheck = state.enemies[index];
        //      else
        //          toCheck = state.allies[index - 3];
        //      if (toCheck == null)
        //          return false;
        //return ((float)toCheck.Curr_hp / (float)toCheck.Stats.max_hp) < healthRatio;
        return false;
    }
}
