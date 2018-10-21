using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BIEnemyIsStunned : BattleInterruptTrigger
{
    public override bool checkTrigger(Battlefield state)
    {
        foreach (CastResults d in state.lastCast)
			if (d.isStun && d.target.CasterType == ICasterType.ENEMY) return true;
        return false;
    }
}
