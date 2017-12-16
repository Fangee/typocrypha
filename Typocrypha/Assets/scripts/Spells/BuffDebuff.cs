using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffDebuff
{
    public BuffDebuff()
    {
        for(int i = 0; i < Elements.count; i++)
        {
            vsElem[i] = 0;
        }
    }
    public void reset()
    {
        attack = 0;
        defense = 0;
        speed = 0;
        accuracy = 0;
        evasion = 0;
        for (int i = 0; i < Elements.count; i++)
        {
            vsElem[i] = 0;
        }
    }

    //private constants
    public const int maxlevel = 4;
    public const int minlevel = maxlevel * -1;
    private const float maxPercent = 2.0F;
    private const float minPercent = 0.5F;
    private const float posLevelMod = maxPercent / maxlevel;
    private const float negLevelMod = minPercent / minlevel;

    //private fields
    int attack = 0;
    int defense = 0;
    int speed = 0;
    int accuracy = 0;
    int evasion = 0;
    int[] vsElem = new int[Elements.count];

    //get/set properties
    public float Attack
    {
        get
        {
            return getMod(attack);
        }

        set
        {
            attack = Mathf.FloorToInt(Mathf.Clamp(value, minlevel, maxlevel));
        }
    }
    public float Defense
    {
        get
        {
            return getMod(defense);
        }

        set
        {
            defense = Mathf.FloorToInt(Mathf.Clamp(value, minlevel, maxlevel));
        }
    }
    public float Speed
    {
        get
        {
            return getMod(speed * -1);
        }

        set
        {
            speed = Mathf.FloorToInt(Mathf.Clamp(value, minlevel, maxlevel));
        }
    }
    public float Accuracy
    {
        get
        {
            return getMod(accuracy);
        }

        set
        {
            accuracy = Mathf.FloorToInt(Mathf.Clamp(value, minlevel, maxlevel));
        }
    }
    public float Evasion
    {
        get
        {
            return getMod(evasion);
        }

        set
        {
            evasion = Mathf.FloorToInt(Mathf.Clamp(value, minlevel, maxlevel));
        }
    }

    //work with elemental weak/res modifiers
    public float modElementState(float curr, int element)
    {
        if (vsElem[element] == 0)
            return curr;
        Elements.vsElement level = Elements.getLevel(curr);
        return Elements.getFloat(Elements.modLevel(level, vsElem[element]));
    }
    public Elements.vsElement modElementLevel(float curr, int element)
    {
        Elements.vsElement level = Elements.getLevel(curr);
        return Elements.modLevel(level, vsElem[element]);
    }
    public void setElementLevel(int element, int level)
    {
        vsElem[element] = Mathf.Clamp(level, minlevel, maxlevel);
    }
    public int getElementLevel(int element)
    {
        return vsElem[element];
    }
    public void addSubElementLevel(int element, int mod)
    {
        vsElem[element] = Mathf.Clamp(vsElem[element] - mod, minlevel, maxlevel);
    }
    public void modify(BuffData mod)
    {
        Attack = attack + mod.attackMod;
        Defense = defense + mod.defenseMod;
        Speed = speed + mod.speedMod;
        Accuracy = accuracy + mod.accuracyMod;
        Evasion = evasion + mod.evasionMod;
        for(int i = 0; i < Elements.count; i++)
        {
            addSubElementLevel(i, mod.vsElemMod[i]);
        }
    }

    private float getMod(int level)
    {
        if (level < 0)
            return 1 - (level * negLevelMod);
        else
            return 1 + (level * posLevelMod);
    }
}
//class containing data on how to modify a buff/debuff state (used in spells)
public class BuffData
{
    public BuffData()
    {
        for (int i = 0; i < Elements.count; i++)
            vsElemMod[i] = 0;
    }
    public BuffData(BuffData toCopy)
    {
        attackMod = toCopy.attackMod;
        defenseMod = toCopy.defenseMod;
        speedMod = toCopy.speedMod;
        accuracyMod = toCopy.accuracyMod;
        evasionMod = toCopy.evasionMod;
        for (int i = 0; i < Elements.count; i++)
            vsElemMod[i] = toCopy.vsElemMod[i];
    }

    //Public fields
    public int attackMod = 0;
    public int defenseMod = 0;
    public int speedMod = 0;
    public int accuracyMod = 0;
    public int evasionMod = 0;
    public int[] vsElemMod = new int[Elements.count];
    //Makes all debuffs into buff (for absorb)
    public void makeBuff()
    {
        if(attackMod < 0)
            attackMod *= -1;
        if(defenseMod < 0)
            defenseMod *= -1;
        if(speedMod < 0)
            speedMod *= -1;
        if(accuracyMod < 0)
            accuracyMod *= -1;
        if(evasionMod < 0)
            evasionMod *= -1;
        for(int i = 0; i < Elements.count; i++)
        {
            if(vsElemMod[i] < 0)
                vsElemMod[i] *= -1;
        }
    }
    public BuffData multiply(int mod)
    {
        attackMod = Utility.Math.Clamp(attackMod * mod, BuffDebuff.minlevel, BuffDebuff.maxlevel); 
        defenseMod = Utility.Math.Clamp(defenseMod * mod, BuffDebuff.minlevel, BuffDebuff.maxlevel); 
        speedMod = Utility.Math.Clamp(speedMod * mod, BuffDebuff.minlevel, BuffDebuff.maxlevel);
        accuracyMod = Utility.Math.Clamp(accuracyMod * mod, BuffDebuff.minlevel, BuffDebuff.maxlevel);
        evasionMod = Utility.Math.Clamp(evasionMod * mod, BuffDebuff.minlevel, BuffDebuff.maxlevel);
        for (int i = 0; i < Elements.count; i++)
        {
            vsElemMod[i] = Utility.Math.Clamp(vsElemMod[i] * mod, BuffDebuff.minlevel, BuffDebuff.maxlevel);
        }
        return this;
    }
}


