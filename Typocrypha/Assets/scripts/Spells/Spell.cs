using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class that holds the basic data for any spell (root keyword)
//A spell must inherit from this class to define specific functionality
public abstract class Spell
{
    public abstract void cast(Enemy[] targets, int selected, Player p);

    public abstract void enemyCast(Enemy[] allies, int position, Player p);

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
  
    public void copyInto(Spell s)
    {
        s.power = power;
        s.cooldown = cooldown;
        s.hitPercentage = hitPercentage;
        s.elementEffectMod = elementEffectMod;
        s.element = element;
        targets.CopyTo(s.targets, 0);
    }

    public string description;
    public int power;
    public int cooldown;
    public int hitPercentage;
    public int elementEffectMod;
    public string element = "null";
    public string type = "null";
    //Targets: {R,M,L,Player}
    public bool[] targets = { false, false, false, false, false };

}
//Spells that attempt to do damage to opposing entities (CURRENTLY INCOMPLETE)
public class AttackSpell : Spell
{
    public override void cast(Enemy[] targets, int selected, Player p)
    {
        Debug.Log("Attack spell cast");
        int damage = power + p.Attack - targets[selected].getStats().defense;
        targets[selected].damage(damage);
        return;
    }

    public override void enemyCast(Enemy[] allies, int position, Player p)
    {
        int damage = power - p.Defense;
        p.damage(damage, element);
        return;
    }
}
//Spells that attempt to heal friendly entities (CURRENTLY INCOMPLETE)
public class HealSpell : Spell
{
    public override void cast(Enemy[] targets, int selected, Player p)
    {
        Debug.Log("Heal spell cast");
        return;
    }

    public override void enemyCast(Enemy[] allies, int position, Player p)
    {
        throw new System.NotImplementedException();
    }
}
//Spells that attempt to shield friendly entities (CURRENTLY INCOMPLETE)
public class ShieldSpell : Spell
{
    public override void cast(Enemy[] targets, int selected, Player p)
    {
        Debug.Log("Shield spell cast");
        return;
    }

    public override void enemyCast(Enemy[] allies, int position, Player p)
    {
        throw new System.NotImplementedException();
    }
}
//Contains the data associated with an Element keyword
public class ElementMod
{
    public string element;
    public int cooldownMod;

}
//Contains the data associated with a Style keyword
public class StyleMod
{
    public int powerMod;
    public int cooldownMod;
    public int accMod;
    public int statusEffectChanceMod;
    public bool isTarget = false;
    public bool[] targets = { false, false, false, false, false };
}