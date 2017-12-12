using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Defines static operations for ICaster children
public static class CasterOps
{
    //Applies damage formula base on base_power (spell strength) and stats of caster and target)
    //damages ref stats appropriately (will not go below zero)
    //Precondition: target.Stats.vsElement[element] != Elements.reflect
    public static bool calcDamage(int base_power, int element, ICaster caster, ICaster target, ref int curr_hp, ref int curr_shield, ref int curr_stagger, bool is_stunned = false)
    {
        float dMod = base_power;
        int staggerDamage = 0;

        //Apply buff/debuffs here

        //Apply stat mods here

        //Absorb damage if enemy absorbs this type
        if (target.Stats.vsElement[element] == Elements.absorb)
        {
            Debug.Log(target.Stats.name + " absorbs " + dMod + " " + Elements.toString(element) + " damage");
            if (curr_hp + Mathf.CeilToInt(dMod) > target.Stats.max_hp)
                curr_hp = target.Stats.max_hp;
            else
                curr_hp += Mathf.CeilToInt(dMod);
            return false;
        }

        //Apply crit here

        //Apply elemental weakness/resistances
        dMod *= target.Stats.vsElement[element];
        if (target.Stats.vsElement[element] > 1)//If enemy is weak
            staggerDamage++;

        //Apply stun damage mod (if stunned)
        if (is_stunned)
            dMod *= (1.25F + (0.25F * staggerDamage));
        Debug.Log(target.Stats.name + " was hit for " + dMod + " of " + Elements.toString(element) + " damage x" + target.Stats.vsElement[element]);
        //Apply shield
        if (curr_shield > 0)
        {
            if (curr_shield - dMod < 0)//Shield breaks
            {
                curr_shield = 0;
                curr_hp -= Mathf.FloorToInt(dMod - curr_shield);
                if (staggerDamage >= 1 && is_stunned == false)
                    curr_stagger--;
            }
            else
                curr_shield -= Mathf.FloorToInt(dMod);
        }
        else
        {
            curr_hp -= Mathf.CeilToInt(dMod);
            //Stagger if enemy is actually damaged
            if (staggerDamage >= 1 && !is_stunned && dMod > 0)
                curr_stagger--;
        }
        if (curr_hp < 0)
            curr_hp = 0;
        return dMod > 0;
    }
}
