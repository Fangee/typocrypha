using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A BattleEventManager manages (checks and triggers) a group of battleEvents
public class BattleEventManager : BattleEventTrigger
{
    public BattleEventTrigger[] BattleEvents;
    List<BattleEventTrigger> nonTriggeredEvents = new List<BattleEventTrigger>();
    List<BattleEventTrigger> toTrigger = new List<BattleEventTrigger>();

    private void Start()
    {
        nonTriggeredEvents.AddRange(BattleEvents);
    }

    public override bool checkTrigger(BattleField state)
    {
        bool ret = false;
        foreach (BattleEventTrigger e in nonTriggeredEvents)
        {
            if (!e.HasTriggered && e.checkTrigger(state))
            {
                toTrigger.Add(e);
                ret = true;
            }
        }
        return ret;
    }
    public override bool onTrigger(BattleField state)
    {
        foreach(BattleEventTrigger e in toTrigger)
        {
            e.onTrigger(state);
            if (e.HasTriggered)
                nonTriggeredEvents.Remove(e);
        }
        if(toTrigger.Count > 0)
        {
            toTrigger.Clear();
            return true;
        }
        return false;
    }
}
