using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BEDoppelFrenzy :BattleEventTrigger {
    public override bool checkTrigger(Battlefield state)
    {
        return PlayerDataManager.main.getData("doppel-frenzy") == "true";
    }

    public override bool onTrigger(Battlefield state)
    {
        BattleManagerS.main.startFrenzyCast();
        return false;
    }
}
