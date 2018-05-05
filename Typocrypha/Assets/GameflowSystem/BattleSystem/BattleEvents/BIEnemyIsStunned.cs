using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BIEnemyIsStunned : BattleInterruptTrigger
{
    public override bool checkTrigger(BattleField state)
    {
        foreach (CastData d in state.last_cast)
			if (d.isStun) return true;
        return false;
    }
}
