using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BIImmediate : BattleInterruptTrigger
{
    public override bool checkTrigger(BattleField state)
    {
        return true;
    }
}
