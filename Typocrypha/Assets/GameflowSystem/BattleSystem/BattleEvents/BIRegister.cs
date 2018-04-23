using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BIRegister : BattleInterruptTrigger
{
    public override bool checkTrigger(BattleField state)
    {
        if (state.last_register[0] || state.last_register[1] || state.last_register[2])
        {
            return true;
        }
        return false;
    }
}
