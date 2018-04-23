using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BICheckHealth : BattleInterruptTrigger {
    public BattleField.FieldPosition whoToCheck;
    public float healthRatio;

    public override bool checkTrigger(BattleField state)
    {
        int index = (int)whoToCheck;
        ICaster toCheck = null;
        if (index < 3)
            toCheck = state.enemy_arr[index];
        else
            toCheck = state.player_arr[index - 3];
        if (toCheck == null)
            return false;
        return (toCheck.Curr_hp / toCheck.Stats.max_hp) < healthRatio;
    }
}
