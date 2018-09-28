using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Defines static operations for ICaster children
public static class CasterOps
{
    //Applies repel and prints appropriate debug log
    //Returns true if reflected, false if not.
    //MAY need fixing for targeting
    public static bool calcRepel(CastData data, int d, int element, ICaster caster, ICaster target, bool crit, bool repel)
    {
        //repel damage to caster if enemy reflects this element (FIX for targeting)
        if (target.Stats.vsElement[element] == Elements.repel && repel == false)
        {
            Debug.Log(target.Stats.name + " reflects " + d + " " + Elements.toString(element) + " damage back at " + caster.Stats.name);
            data.repel = true;
            caster.damage(data, d, element, caster, crit, true);
            return true;
        }
        return false;
    }
    //Applies damage formula base on base_power (spell strength) and stats of caster and target)
    //damages ref stats appropriately (will not go below zero)
    //Precondition: target.Stats.vsElement[element] != Elements.repel
    public static bool calcDamage(CastData data, int base_power, int element, ICaster caster, ICaster target, bool crit, bool is_stunned = false)
    {
        float dMod = base_power;
        int staggerDamage = 0;

        //Apply buff/debuffs here (NOT DONE)
        CasterStats casterMod = applyBuff(caster);
        CasterStats targetMod = applyBuff(target);

        //Apply attack bonus formula here
        dMod *= casterMod.attack;

        //Absorb damage if enemy absorbs this type
        if (targetMod.vsElement[element] == Elements.drain)
        {
            Debug.Log(target.Stats.name + " absorbs " + dMod + " " + Elements.toString(element) + " damage");
            if (target.Curr_hp + Mathf.CeilToInt(dMod) > target.Stats.max_hp)
                target.Curr_hp = target.Stats.max_hp;
            else
                target.Curr_hp += Mathf.CeilToInt(dMod);

            data.damageInflicted = Mathf.CeilToInt(-1 * dMod);
            data.vsElement = Elements.vsElement.DRAIN;
            return false;
        }

        //Subtract enemy defense penalty here
        dMod = dMod - (dMod * targetMod.defense);

        //Add random modifier here
        dMod *= Random.Range(0.9F, 1.1F);

        //Apply crit here
        if (crit)
            staggerDamage++;

        //Apply elemental weakness/resistances
        dMod *= targetMod.vsElement[element];
        if (targetMod.vsElement[element] == 0.0F)
            data.vsElement = Elements.vsElement.BLOCK;
        else if (targetMod.vsElement[element] > 1)//If enemy is weak
        {
            if (targetMod.vsElement[element] > 2)
                data.vsElement = Elements.vsElement.SUPERWEAK;
            else
                data.vsElement = Elements.vsElement.WEAK;
            staggerDamage++;
        }
        else if (targetMod.vsElement[element] < 1)
            data.vsElement = Elements.vsElement.RESIST;
        else
            data.vsElement = Elements.vsElement.NEUTRAL;

        //Apply stun damage mod (if stunned)
        if (is_stunned)
            dMod *= (1.25F + (0.25F * staggerDamage));
        else
        {
            target.Curr_hp -= Mathf.CeilToInt(dMod);
            //Stagger if enemy is actually damaged
            if (staggerDamage >= 1 && !is_stunned && dMod > 0)
                target.Curr_stagger--;
        }
        if (target.Curr_hp < 0)
            target.Curr_hp = 0;
		if (target.Curr_hp > target.Stats.max_hp)
			target.Curr_hp = target.Stats.max_hp;
        data.damageInflicted = Mathf.CeilToInt(dMod);
        return dMod > 0;
    }
    //Applies ICaster's buff/debuff state to its stats
    public static CasterStats applyBuff(ICaster caster)
    {
        return caster.Stats.modify(caster.BuffDebuff);
    }
}
