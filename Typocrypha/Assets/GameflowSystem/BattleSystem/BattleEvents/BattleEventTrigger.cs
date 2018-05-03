using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleEventTrigger : MonoBehaviour {
    protected bool triggered = false;
    public virtual bool HasTriggered { get { return triggered; } set { triggered = value; } }
    //Returns true if the trigger conditions are met, else false
    public abstract bool checkTrigger(BattleField state);
    //Returns true if the triggering should pause the battle (i.e interrupts) else false
    public abstract bool onTrigger(BattleField state);
}
