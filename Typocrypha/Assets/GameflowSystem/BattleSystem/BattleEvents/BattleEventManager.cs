using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A BattleEventManager manages (checks and triggers) a group of battleEvents
public class BattleEventManager : BattleEventTrigger
{
    public BattleEventTrigger[] BattleEvents;
    List<BattleEventTrigger> nonTriggeredEvents = new List<BattleEventTrigger>();
    BattleEventTrigger toTrigger = null;

    private void Start()
    {
        nonTriggeredEvents.AddRange(BattleEvents);
    }

    public override bool checkTrigger(BattleField state)
    {
        foreach (BattleEventTrigger e in nonTriggeredEvents)
        {
            if (!e.HasTriggered && e.checkTrigger(state))
            {
                toTrigger = e;
                return true;
            }
        }
        return false;
    }
    public override bool onTrigger(BattleField state)
    {
        bool ret = toTrigger.onTrigger(state);
        if(toTrigger.HasTriggered)
            nonTriggeredEvents.Remove(toTrigger);
        return ret;
    }
}
