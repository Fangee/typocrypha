using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BISpecificAttack : BattleInterruptTrigger
{
    public BattleField.FieldPosition target;
    public Elements.Element elementMustBe = Elements.Element.ANY;
    public bool spellMustHit = true;
    public bool spellMustCrit = false;
    public bool spellMustStun = false;
    public int damageMustBeAtLeast = 0;
    public Elements.vsElement vsElementMustBe = Elements.vsElement.ANY;

    public override bool checkTrigger(BattleField state)
    {
        int index = (int)target;
        ICaster toCheck = null;
        if (index < 3)
            toCheck = state.enemy_arr[index];
        else
            toCheck = state.player_arr[index - 3];
        if (state.last_cast != null)
        {
            foreach (CastData d in state.last_cast)
            {
                if (d.Target == toCheck)
                {
                    bool element = (elementMustBe == Elements.Element.ANY) || ((int)elementMustBe == d.element);
                    bool hit = (!spellMustHit) || d.isHit;
                    bool crit = (!spellMustCrit) || d.isCrit;
                    bool stun = (!spellMustStun) || d.isStun;
                    bool dmg = d.damageInflicted >= damageMustBeAtLeast;
                    bool elemVs = (vsElementMustBe == Elements.vsElement.ANY) || (vsElementMustBe == d.elementalData);
                    return (element && hit && crit && stun && dmg && elemVs);
                }
            }
        }
        return false;
    }
}
