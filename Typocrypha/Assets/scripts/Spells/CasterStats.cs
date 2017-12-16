using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class with all read-only value that defines the necessary stats for any caster
public class CasterStats
{
    //Sets all read-only values
    public CasterStats(string name, int hp, int shield, int stagger, float atk, float def, float speed, float acc, int evade, float[] vsElem = null)
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
    public readonly float accuracy;      //numerical hitchance boost
    public readonly int evasion;       //numerical dodgechance boost
    public readonly float[] vsElement; //elemental weaknesses/resistances

    //Return the equivalent of this modified by debuff mod
    public CasterStats modify(BuffDebuff mod)
    {
        float atk = attack * mod.Attack;
        float def = defense * mod.Defense;
        float spd = speed * mod.Speed;
        float acc = accuracy * mod.Accuracy;
        int evade = Mathf.FloorToInt(evasion * mod.Evasion);
        float[] vE;
        vE = new float[Elements.count];

        for (int i = 0; i < Elements.count; i++) 
        {
            vE[i] = mod.modElementState(vsElement[i], i);
        }
            
        return new CasterStats(name, max_hp, max_shield, max_stagger, atk, def, spd, acc, evade, vE);
    }
    //Just get debuff modified ACCURACY
    public float getModAcc(BuffDebuff mod)
    {
        return accuracy * mod.Accuracy;
    }
    //Just get debuff modified EVADE
    public int getModEvade(BuffDebuff mod)
    {
        return Mathf.FloorToInt(evasion * mod.Evasion);
    }
    public Elements.vsElement getModVsElement(BuffDebuff mod, int element)
    {
        return mod.modElementLevel(vsElement[element], element);
    }
    public float getFloatVsElement(BuffDebuff mod, int element)
    {
        return mod.modElementState(vsElement[element], element);
    }
}
