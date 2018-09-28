using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Maintains a list of battleEvents and triggers if the chosen logical operator 
//returns true on the HasTriggered values of the operands
public class BELogicTrigger : BattleEventTrigger {
    public BattleEventTrigger EventToTrigger;
    public Operators.LogicalOperator LogicalOperator;
    public BattleEventTrigger[] Operands;

    public override bool checkTrigger(Battlefield state)
    {
        switch (LogicalOperator)
        {
            case Operators.LogicalOperator.NOT:
                foreach (BattleEventTrigger t in Operands)
                    return !t.HasTriggered;
                throw new System.Exception("BELogicTrigger: Logical Error, NOT requires an operand");
            case Operators.LogicalOperator.OR:
                foreach (BattleEventTrigger t in Operands)
                    if (t.HasTriggered)
                        return true;
                return false;
            case Operators.LogicalOperator.AND:
                foreach (BattleEventTrigger t in Operands)
                    if (!t.HasTriggered)
                        return false;
                return true;
            case Operators.LogicalOperator.NOR:
                foreach (BattleEventTrigger t in Operands)
                    if (t.HasTriggered)
                        return false;
                return true;
            case Operators.LogicalOperator.NAND:
                foreach (BattleEventTrigger t in Operands)
                    if (!t.HasTriggered)
                        return true;
                return false;
            case Operators.LogicalOperator.XOR:
                bool atLeastOneIsTrue = false;
                foreach (BattleEventTrigger t in Operands)
                {
                    if (t.HasTriggered)
                    {
                        if (atLeastOneIsTrue)
                            return false;
                        atLeastOneIsTrue = true;
                    }
                }
                return atLeastOneIsTrue; //if true, one and only one is true at this point
            case Operators.LogicalOperator.XNOR:
                bool atLeastOneIsFalse = false;
                foreach (BattleEventTrigger t in Operands)
                {
                    if (!t.HasTriggered)
                    {
                        if (atLeastOneIsFalse)
                            return false;
                        atLeastOneIsFalse = true;
                    }
                }
                return atLeastOneIsFalse; //if true, one and only one is false at this point
        }
        throw new System.Exception("BELogicTrigger: Logical Error, switch covers all cases");
    }

    public override bool onTrigger(Battlefield state)
    {
        triggered = true;
        return EventToTrigger.onTrigger(state);
    }
}
