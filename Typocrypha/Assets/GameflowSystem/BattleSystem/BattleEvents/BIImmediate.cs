using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BIImmediate : BattleInterruptTrigger
{
    public override bool checkTrigger(Battlefield state)
    {
        return true;
    }
}
