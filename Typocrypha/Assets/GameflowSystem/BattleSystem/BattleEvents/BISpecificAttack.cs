using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BISpecificAttack : BattleInterruptTrigger
{
    public BattleField.FieldPosition caster = BattleField.FieldPosition.ANY;
    public BattleField.FieldPosition target = BattleField.FieldPosition.ANY;
    public string rootKeywordIs = "";
    public string elementKeywordIs = "";
    public string styleKeywordIs = "";
    public Elements.Element elementMustBe = Elements.Element.ANY;
    public bool spellMustHit = true;
    public bool spellMustCrit = false;
    public bool spellMustStun = false;
    public int damageIsAtLeast = 0;
    public Elements.vsElement vsElementMustBe = Elements.vsElement.ANY;

    public override bool checkTrigger(BattleField state)
    {
        if(state.last_spell == null || 
          (rootKeywordIs != "" && rootKeywordIs != state.last_spell.root) || 
          (elementKeywordIs != "" && elementKeywordIs != state.last_spell.element) ||
          (styleKeywordIs != "" && styleKeywordIs != state.last_spell.style))
            return false;
        if (state.last_cast == null || state.last_cast.Count <= 0)
            return false;
        if (caster != BattleField.FieldPosition.ANY && state.last_cast[0].Caster != state.getCasterFromFieldPosition(caster))
            return false;
        ICaster targetToCheck = state.getCasterFromFieldPosition(target);
        if (targetToCheck == null && target != BattleField.FieldPosition.ANY)
            return false;
        foreach (CastData d in state.last_cast)
        {
            if (target == BattleField.FieldPosition.ANY || d.Target == targetToCheck)
            {
                bool element = (elementMustBe == Elements.Element.ANY) || ((int)elementMustBe == d.element);
                bool hit = (!spellMustHit) || d.isHit;
                bool crit = (!spellMustCrit) || d.isCrit;
                bool stun = (!spellMustStun) || d.isStun;
                bool dmg = d.damageInflicted >= damageIsAtLeast;
                bool elemVs = (vsElementMustBe == Elements.vsElement.ANY) || (vsElementMustBe == d.elementalData);
                bool result = (element && hit && crit && stun && dmg && elemVs);
                if (result)
                    return true;
                else if (target == BattleField.FieldPosition.ANY)
                    continue;
                return false;
            }
        }
        return false;
    }
}
