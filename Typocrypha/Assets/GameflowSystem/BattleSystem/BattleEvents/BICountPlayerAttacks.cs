using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BICountPlayerAttacks : BattleInterruptTrigger {
    //number of attacks to trigger after
    public int triggerAfterXAttacks;
    public override bool checkTrigger(Battlefield state)
    {
        return state.num_player_attacks >= triggerAfterXAttacks;
    }
}
