using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BISpecificAttack : BattleInterruptTrigger
{
    public int numAttacks = 1;
    public BattleField.FieldPosition caster = BattleField.FieldPosition.ANY;
    public BattleField.FieldPosition target = BattleField.FieldPosition.ANY;
    public string rootKeywordIs = string.Empty;
    public string elementKeywordIs = string.Empty;
    public string styleKeywordIs = string.Empty;
    public Elements.Element elementMustBe = Elements.Element.ANY;
    public bool spellMustHit = true;
    public bool spellMustCrit = false;
    public bool spellMustStun = false;
    public int damageIsAtLeast = 0;
    public Elements.vsElement vsElementMustBe = Elements.vsElement.ANY;

    private int curr_attacks = 0;

    public override bool checkTrigger(BattleField state)
    {
        bool ret = false;
        if (caster == BattleField.FieldPosition.PLAYER)
            ret = checkCast(state, state.last_player_cast, state.last_player_spell);
        else if (caster == BattleField.FieldPosition.LEFT || caster == BattleField.FieldPosition.MIDDLE || caster == BattleField.FieldPosition.RIGHT)
            ret = checkCast(state, state.last_enemy_cast, state.last_enemy_spell);
        else if (caster == BattleField.FieldPosition.ANY)
            ret = checkCast(state, state.last_player_cast, state.last_player_spell) || checkCast(state, state.last_enemy_cast, state.last_enemy_spell);
        return (ret && ++curr_attacks >= numAttacks);
    }
    protected bool checkCast(BattleField state, List<CastData> dataToCheck, SpellData spellToCheck)
    {
        if (spellToCheck == null ||
            (!string.IsNullOrEmpty(rootKeywordIs) && rootKeywordIs != spellToCheck.root) ||
            (!string.IsNullOrEmpty(elementKeywordIs) && elementKeywordIs != spellToCheck.element) ||
            (!string.IsNullOrEmpty(styleKeywordIs) && styleKeywordIs != spellToCheck.style))
            return false;
        if (dataToCheck == null || dataToCheck.Count <= 0)
            return false;
        if (caster != BattleField.FieldPosition.ANY && dataToCheck[0].Caster != state.getCasterFromFieldPosition(caster))
            return false;
        ICaster targetToCheck = state.getCasterFromFieldPosition(target);
        if (targetToCheck == null && target != BattleField.FieldPosition.ANY)
            return false;
        foreach (CastData d in dataToCheck)
        {
            if (target == BattleField.FieldPosition.ANY || d.Target == targetToCheck)
            {
                bool element = (elementMustBe == Elements.Element.ANY) || ((int)elementMustBe == d.element || (elementMustBe == Elements.Element.NOTNULL && d.element != Elements.@null));
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
