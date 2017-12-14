using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Defines static operations for ICaster children
public static class CasterOps
{
    //Applies damage formula base on base_power (spell strength) and stats of caster and target)
    //damages ref stats appropriately (will not go below zero)
    //Precondition: target.Stats.vsElement[element] != Elements.reflect
    public static bool calcDamage(CastData data, int base_power, int element, ICaster caster, ICaster target, bool crit, bool is_stunned = false)
    {
        float dMod = base_power;
        int staggerDamage = 0;

        //Apply buff/debuffs here (NOT DONE)
        CasterStats casterMod = caster.Stats;
        CasterStats targetMod = target.Stats;

        //Apply attack bonus formula here
        dMod *= casterMod.attack;

        //Absorb damage if enemy absorbs this type
        if (target.Stats.vsElement[element] == Elements.absorb)
        {
            Debug.Log(target.Stats.name + " absorbs " + dMod + " " + Elements.toString(element) + " damage");
            if (target.Curr_hp + Mathf.CeilToInt(dMod) > target.Stats.max_hp)
                target.Curr_hp = target.Stats.max_hp;
            else
                target.Curr_hp += Mathf.CeilToInt(dMod);

            data.damageInflicted = Mathf.CeilToInt(-1 * dMod);
            data.elementalData = CastData.vsElement.ABSORB;
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
        dMod *= target.Stats.vsElement[element];
        if (target.Stats.vsElement[element] > 1)//If enemy is weak
        {
            data.elementalData = CastData.vsElement.WEAK;
            staggerDamage++;
        }
        else if(target.Stats.vsElement[element] < 1)
            data.elementalData = CastData.vsElement.RESISTANT;
        else
            data.elementalData = CastData.vsElement.NEUTERAL;

        //Apply stun damage mod (if stunned)
        if (is_stunned)
            dMod *= (1.25F + (0.25F * staggerDamage));
        //Apply shield
        if (target.Curr_shield > 0)
        {
            if (target.Curr_shield - dMod < 0)//Shield breaks
            {
                target.Curr_shield = 0;
                target.Curr_hp -= Mathf.CeilToInt(dMod - target.Curr_shield);
                if (staggerDamage >= 1 && is_stunned == false)
                    target.Curr_stagger--;
            }
            else
                target.Curr_shield -= Mathf.CeilToInt(dMod);
        }
        else
        {
            target.Curr_hp -= Mathf.CeilToInt(dMod);
            //Stagger if enemy is actually damaged
            if (staggerDamage >= 1 && !is_stunned && dMod > 0)
                target.Curr_stagger--;
        }
        if (target.Curr_hp < 0)
            target.Curr_hp = 0;
        data.damageInflicted = Mathf.CeilToInt(dMod);
        return dMod > 0;
    }
}
