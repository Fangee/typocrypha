using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class that holds the basic data for any spell (root keyword)
//A spell must inherit from this class to define specific functionality
public abstract class Spell
{
    //Casts this spell at selected target (at targets[selected])
    public abstract void cast(ICaster[] targets, int selected, ICaster[]allies, int position);
    //Starts spell cooldown using coroutine support from the Timer class 
    public void startCooldown(CooldownList l, string name, float time)
    {
        finish_time = time;
        l.add(name, time, curr_time, isNotOnCooldown);
    }

    //Apllies prefix and suffix to spell. both arguments can be null (if no prefix or suffix)
    public void Modify(ElementMod e, StyleMod s)
    {       
        if(e != null)//Add element modifier
        {
            element = e.element;
            cooldown += e.cooldownMod;
        }
        if(s != null)//Add style modifier
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
    //Helper method to copy data from one spell into another (s must be same type as this)
    //ONLY USE IN SPELLDICTIONARY
    public void copyInto(Spell s)
    {
        s.power = power;
        s.cooldown = cooldown;
        s.hitPercentage = hitPercentage;
        s.elementEffectMod = elementEffectMod;
        s.element = element;
        targets.CopyTo(s.targets, 0);
    }

    //public fields

    public string description;          //Spell's description (in spellbook)
    public int power;                   //Spell's intensity (not necessarily just damage)
    public float cooldown;              //Spell's base cooldown
    public int hitPercentage;           //Spell's base hit chance (1 = 1%)
    public int elementEffectMod;        //Spell's base elemental effect chance (1 = 1%)
    public int element = Elements.@null;     //Spell's elemental damage type
    public string type = "null";        //Spell's effect type (attack, shield, heal, etc.)
    //Targets: {R,M,L,Self,CursorDependent?}
    public bool[] targets = { false, false, false, false, false };

    //Cooldown properties

    //bool ref for passing to CooldownList
    Ref<bool> isNotOnCooldown = new Ref<bool>(true);
    //Returns true is on cooldown, false otherwise
    public bool IsOnCooldown
    {
        get
        {
            return !isNotOnCooldown.Obj;
        }

        set
        {
            isNotOnCooldown.Obj = !value;
        }
    }
    //float ref for passing to CooldownList
    Ref<float> curr_time = new Ref<float>(0.0F);
    //How many seconds to finish spell
    float finish_time = 0.0F;
    public float TimeLeft
    {
        get { return finish_time - curr_time.Obj; }
    }
}
//Spells that attempt to do damage to opposing entities (Add targeting)
public class AttackSpell : Spell
{
    public override void cast(ICaster[] targets, int selected, ICaster[] allies, int position)
    {
        ICaster caster = allies[position];
        targets[selected].damage(power, element, caster);
        return;
    }
}
//Spells that attempt to heal friendly entities (CURRENTLY INCOMPLETE)
public class HealSpell : Spell
{
    public override void cast(ICaster[] targets, int selected, ICaster[] allies, int position)
    {
        throw new System.NotImplementedException();
    }
}
//Spells that attempt to shield friendly entities (CURRENTLY INCOMPLETE)
public class ShieldSpell : Spell
{
    public override void cast(ICaster[] targets, int selected, ICaster[] allies, int position)
    {
        throw new System.NotImplementedException();
    }
}
//Contains the data associated with an Element keyword
public class ElementMod
{
    public int element;      //Elemental modifier to apply
    public float cooldownMod;

}
//Contains the data associated with a Style keyword
public class StyleMod
{
    public int powerMod;
    public float cooldownMod;
    public int accMod;
    public int statusEffectChanceMod;
    public bool isTarget = false;
    public bool[] targets = { false, false, false, false, false };
}