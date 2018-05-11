using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BIRegister : BattleInterruptTrigger
{
    public Spell.WordType wordType = Spell.WordType.ANY;
    public string wordMustBe = string.Empty;
    public override bool checkTrigger(BattleField state)
    {
        switch (wordType)
        {
            case Spell.WordType.ANY:
                if((state.last_register[0] || state.last_register[1] || state.last_register[2]))
                {
                    if (string.IsNullOrEmpty(wordMustBe))
                        return true;
                    return (state.last_spell.root == wordMustBe) || (state.last_spell.element == wordMustBe) || (state.last_spell.style == wordMustBe);
                }
                return false;
            case Spell.WordType.ROOT:
                if (state.last_register[1])
                {
                    if (string.IsNullOrEmpty(wordMustBe))
                        return true;
                    return wordMustBe == state.last_spell.root;
                }
                return false;
            case Spell.WordType.ELEMENT:
                if (state.last_register[0])
                {
                    if (string.IsNullOrEmpty(wordMustBe))
                        return true;
                    return wordMustBe == state.last_spell.element;
                }
                return false;
            case Spell.WordType.STYLE:
                if (state.last_register[2])
                {
                    if (string.IsNullOrEmpty(wordMustBe))
                        return true;
                    return wordMustBe == state.last_spell.style;
                }
                return false;
        }
        return false;
    }
}
