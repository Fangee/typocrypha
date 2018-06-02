using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BEDoppelFrenzy :BattleEventTrigger {
    public override bool checkTrigger(BattleField state)
    {
        return PlayerDialogueInfo.main.getInfo("doppel-frenzy") == "true";
    }

    public override bool onTrigger(BattleField state)
    {
        BattleManagerS.main.startFrenzyCast();
        return false;
    }
}
