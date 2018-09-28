﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BIEnemyIsStunned : BattleInterruptTrigger
{
    public override bool checkTrigger(Battlefield state)
    {
        foreach (CastData d in state.last_enemy_cast)
			if (d.isStun) return true;
        return false;
    }
}
