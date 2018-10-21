using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BIRegister : BattleInterruptTrigger
{
    public Spell.WordType wordType = Spell.WordType.ANY;
    public string wordMustBe = string.Empty;
    public override bool checkTrigger(Battlefield state)
    {
        //switch (wordType)
        //{
        //    case Spell.WordType.ANY:
        //        if((state.lastRegister[0] || state.lastRegister[1] || state.lastRegister[2]))
        //        {
        //            if (string.IsNullOrEmpty(wordMustBe))
        //                return true;
        //            return (state.lastSpell.root == wordMustBe) || (state.lastSpell.element == wordMustBe) || (state.lastSpell.style == wordMustBe) || (state.lastSpell.root == wordMustBe) || (state.lastSpell.element == wordMustBe) || (state.lastSpell.style == wordMustBe);
        //        }
        //        return false;
        //    case Spell.WordType.ROOT:
        //        if (state.lastRegister[1])
        //        {
        //            if (string.IsNullOrEmpty(wordMustBe))
        //                return true;
        //            return (wordMustBe == state.lastSpell.root) || (wordMustBe == state.lastSpell.root);
        //        }
        //        return false;
        //    case Spell.WordType.ELEMENT:
        //        if (state.lastRegister[0])
        //        {
        //            if (string.IsNullOrEmpty(wordMustBe))
        //                return true;
        //            return (wordMustBe == state.lastSpell.element) || (wordMustBe == state.lastSpell.element);
        //        }
        //        return false;
        //    case Spell.WordType.STYLE:
        //        if (state.lastRegister[2])
        //        {
        //            if (string.IsNullOrEmpty(wordMustBe))
        //                return true;
        //            return (wordMustBe == state.lastSpell.style) || (wordMustBe == state.lastSpell.style);
        //        }
        //        return false;
        //}
        return false;
    }
}
