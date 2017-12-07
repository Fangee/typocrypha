using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell
{
    public abstract void cast();

    //Apllies prefix and suffix to spell. both arguments can be null (if no prefix or suffix)
    public void Modify(ElementMod e, StyleMod s)
    {
        if(e != null)
        {
            element = e.element;
            cooldown += e.cooldownMod;
        }
        if(s != null)
        {
            power += s.powerMod;
            cooldown += s.cooldownMod;
            hitPercentage += s.accMod;
            elementEffectMod += s.statusEffectChanceMod;
            if (s.isTarget == true)
            {
                for(int i = 0; i < 5; i++)
                {
                    targets[i] = s.targets[i];
                }
            }
        }
    }

    public string description;
    public int power;
    public int cooldown;
    public int hitPercentage;
    public int elementEffectMod;
    string element = "null";
    //Targets: {R,M,L,Player}
    public bool[] targets = { false, false, false, false, false };

}

public class AttackSpell : Spell
{
    public override void cast()
    {
        return;
    }
}

public class HealSpell : Spell
{
    public override void cast()
    {
        return;
    }
}

public class ShieldSpell : Spell
{
    public override void cast()
    {
        return;
    }
}

public class ElementMod
{
    public string element;
    public int cooldownMod;

}

public class StyleMod
{
    public int powerMod;
    public int cooldownMod;
    public int accMod;
    public int statusEffectChanceMod;
    public bool isTarget = false;
    public bool[] targets = { false, false, false, false, false };
}




