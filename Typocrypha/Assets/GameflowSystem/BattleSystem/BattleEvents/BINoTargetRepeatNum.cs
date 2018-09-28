using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BINoTargetRepeatNum : BattleInterruptRepeatAfterNum {
    public override bool checkTrigger(Battlefield state)
    {
        if (state.lastCaster.CasterType != ICasterType.PLAYER)
            return false;
        return state.lastCast.Count == 0;
    }
}
