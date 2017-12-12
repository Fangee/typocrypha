using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Povides OOP structure neccesary for Universal casting

//Anything that can cast
public interface ICaster
{
    CasterStats Stats { get;}

    void damage(int d, int element, ICaster caster, bool reflect = false);
}
//Defines static operations for ICaster children
public static class CasterOps
{
    //Applies damage formula base on base_power (spell strength) and stats of caster and target)
    //damages ref stats appropriately (will not go below zero)
    //
    //Precondition: target.Stats.vsElement[element] != Elements.reflect
    public static bool calcDamage(int base_power, int element, ICaster caster, ICaster target, ref int curr_hp, ref int curr_shield, ref int curr_stagger, bool is_stunned = false)
    {
        //Add hit/miss here

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
//Class with all read-only value that defines the necessary stats for any caster
public class CasterStats
{
    //Sets all read-only values
    public CasterStats(string name, int hp, int shield, int stagger, float atk, float def, float speed, int acc, int evade, float[] vsElem = null)
    {
        this.name = name;
        max_hp = hp;
        max_shield = shield;
        max_stagger = stagger;
        attack = atk;
        defense = def;
        this.speed = speed;
        accuracy = acc;
        evasion = evade;
        vsElement = vsElem;
    }

    //Readonly fields

    public readonly string name;     //name
    public readonly int max_hp;      //max health
    public readonly int max_stagger; //max stagger
    public readonly int max_shield;  //max shield
    //Spell modifiers 
    public readonly float attack;      //numerical damage boost
    public readonly float defense;     //numerical damage reduction
    public readonly float speed;       //percentage of casting time reduction
    public readonly int accuracy;      //numerical hitchance boost
    public readonly int evasion;       //numerical dodgechance boost
    public readonly float[] vsElement; //elemental weaknesses/resistances
    //Return the equivalent of this + CasterStats mod (to be used for stat buff/debuffs)
    CasterStats modify(CasterStats mod)
    {
        int hp = max_hp + mod.max_hp;
        int shield = max_shield + mod.max_shield;
        int stag = max_stagger + mod.max_stagger;
        float atk = attack + mod.attack;
        float def = defense + mod.defense;
        float spd = speed + mod.speed;
        int acc = accuracy + mod.accuracy;
        int evade = evasion + mod.evasion;
        float[] vE;
        if (mod.vsElement == null)
            vE = null;
        else
        {
            vE = new float[Elements.count];

            for (int i = 0; i < Elements.count; i++)
            {
                vE[i] = vsElement[i] + mod.vsElement[i];
            }
        }
        return new CasterStats(name, hp, shield, stag, atk, def, spd, acc, evade, vE);
    }
}

