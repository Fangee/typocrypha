using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
