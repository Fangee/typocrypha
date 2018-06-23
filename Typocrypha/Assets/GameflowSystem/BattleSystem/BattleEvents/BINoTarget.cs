using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BINoTarget : BattleInterruptTrigger
{
    public override bool checkTrigger(BattleField state)
    {
        if (state.lastCaster != BattleField.FieldPosition.PLAYER)
            return false;
        return state.last_player_cast.Count == 0;
    }
}
