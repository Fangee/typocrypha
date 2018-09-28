using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BINoTarget : BattleInterruptTrigger
{
    public override bool checkTrigger(Battlefield state)
    {
        if (state.lastCaster != Battlefield.FieldPosition.PLAYER)
            return false;
        return state.last_player_cast.Count == 0;
    }
}
