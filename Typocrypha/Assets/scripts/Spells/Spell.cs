using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class that holds the basic data for any spell (root keyword)
//A spell must inherit from this class to define specific functionality
public abstract class Spell
{
    //public methods

    //Casts this spell at selected target (at targets[selected])
    public abstract void cast(ICaster target, ICaster caster);
    //Starts spell cooldown using coroutine support from the Timer class 
    public void startCooldown(CooldownList l, string name, float time)
    {
        finish_time = time;
        l.add(name, time, curr_time, isNotOnCooldown);
    }
    //Apllies prefix and suffix to spell. both arguments can be null (if no prefix or suffix)
    public void Modify(ElementMod e, StyleMod s)
    {
        //Handle Cooldown (sequence matters)
        if (e != null && s != null)
        {
            float baseTime = cooldown;
            cooldown *= e.cooldownModM;
            cooldown += (baseTime * s.cooldownModM) - baseTime;
            cooldown += e.cooldownMod;
            cooldown += s.cooldownMod;
        }
        else if (e != null)
        {
            cooldown *= e.cooldownModM;
            cooldown += e.cooldownMod;
        }
        else if (s != null)
        {
            cooldown *= s.cooldownModM;
            cooldown += s.cooldownMod;
        }
        //Add rest of stuff
        if (e != null)//Add element modifier
        {
            element = e.element;
            name = e.name + "-" + name;
        }
        if(s != null)//Add style modifier
        {
            //Apply power mod
            power = Mathf.CeilToInt(power * s.powerModM);
            power += s.powerMod;
            //Apply acc mod
            hitPercentage = Mathf.CeilToInt(hitPercentage * s.accModM);
            hitPercentage += s.accMod;
            //Appl crit mod
            critPercentage = Mathf.CeilToInt(critPercentage * s.critModM);
            critPercentage += s.critMod;
            //Apply status % mod
            elementEffectMod = Mathf.CeilToInt(elementEffectMod * s.statusEffectChanceModM);
            elementEffectMod += s.statusEffectChanceMod;
            if (s.isTarget == true)
            {
                targetData.modify(s.targets);
            }
            name += ("-" + s.name);
        }
    }
    //Returns target pattern of spell as a List of ICasters
    //Precondition: modify() has been appropriately called on this spell
    public List<ICaster> target(ICaster[] targets, int selected, ICaster[] allies, int position)
    {
        List<ICaster> castAt = new List<ICaster>();
        int i = 1;
        if (targetData.targeted)//If spell is cursor-dependant
            i += (selected - 1);
        if (targetData.enemyM && targets[i] != null && !targets[i].Is_dead)//Middle enemy
            castAt.Add(targets[i]);
        i--;
        if (i >= 0 && targetData.enemyL && targets[i] != null && !targets[i].Is_dead)//Left enemy
            castAt.Add(targets[i]);
        i += 2;
        if (i <= 2 && targetData.enemyR && targets[i] != null && !targets[i].Is_dead)//Right enemy
            castAt.Add(targets[i]);
        i = 1;
        if (targetData.selfCenter)//If spell is not cursor-dependant
            i += (position - 1);
        if (targetData.allyM && allies[i] != null && !allies[i].Is_dead)//Middle ally
            castAt.Add(allies[i]);
        i--;
        if (i >= 0 && targetData.allyL && allies[i] != null && !allies[i].Is_dead)//Left ally
            castAt.Add(allies[i]);
        i += 2;
        if (i <= 2 && targetData.allyR && allies[i] != null && !allies[i].Is_dead)//Right ally
            castAt.Add(allies[i]);
        return castAt;
    }
    //Helper method to copy data from one spell into another (s must be same type as this)
    //ONLY USE IN SPELLDICTIONARY
    public void copyInto(Spell s)
    {
        s.name = name;
        s.power = power;
        s.cooldown = cooldown;
        s.hitPercentage = hitPercentage;
        s.critPercentage = critPercentage;
        s.elementEffectMod = elementEffectMod;
        s.element = element;
        s.targetData = new TargetData(false);
        s.targetData.modify(targetData);
    }

    //protected methods

    //Return true if spell hits target, else false (does not actually apply spell effect)
    //Factors in target stunState if checkStun = true
    //ONLY CALL IN CAST (or after spell has been been properly modified with Modify())
    protected bool hitCheck(ICaster target, ICaster caster, bool checkStun = false)
    {
        if (checkStun && target.Is_stunned)
            return true;
        int chance = Mathf.CeilToInt(hitPercentage * caster.Stats.accuracy) - target.Stats.evasion;
        if((Random.Range(0.0F, 1F) * 100) <= chance)
            return true;
        Debug.Log(caster.Stats.name + " missed " + target.Stats.name + "!");
        return false;
    }
    //Return true if spell crits target, else false. Multiplies power by 1.5 if a hit (round up)
    //Factors in target stunState if checkStun = true
    //ONLY CALL IN CAST (or after spell has been been properly modified with Modify())
    protected bool critCheck(ICaster target, ICaster caster)
    {
        float accBonus = Mathf.Clamp(((caster.Stats.accuracy - 1) * (1 - (target.Stats.evasion * 0.01F))) * 0.2F, 0, 2) * 10;
        float chance = critPercentage + accBonus;
        if ((Random.Range(0.0F, 1F) * 100) <= chance)
        {
            Debug.Log(caster.Stats.name + " scores a critical with " + name + " on " + target.Stats.name);
            return true;
        }
        return false;
    }

    //public fields

    public string name;
    public string description;          //Spell's description (in spellbook)
    public int power;                   //Spell's intensity (not necessarily just damage)
    public float cooldown;              //Spell's base cooldown
    public int hitPercentage;           //Spell's base hit chance (1 = 1%)
    public int critPercentage;          //Spell's base crit chance (1 = 1%)
    public int elementEffectMod;        //Spell's base elemental effect chance (1 = 1%)
    public int element = Elements.@null;     //Spell's elemental damage type
    public string type = "null";        //Spell's effect type (attack, shield, heal, etc.)
    //Targets: {L ,M ,R ,leftAlly, mAlly, rAlly, SelfCenter, CursorDependent?}
    public TargetData targetData;

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
    public override void cast(ICaster target, ICaster caster)
    {
        if(hitCheck(target,caster, true))
        {
            bool crit = critCheck(target, caster);
            int powerMod;
            if (crit)
                powerMod = Mathf.CeilToInt(power * 1.5F);
            else
                powerMod = power;
            target.damage(powerMod, element, caster, crit);
        }
    }
}
//Spells that attempt to heal friendly entities (CURRENTLY INCOMPLETE)
public class HealSpell : Spell
{
    public override void cast(ICaster target, ICaster caster)
    {
        throw new System.NotImplementedException();
    }
}
//Spells that attempt to shield friendly entities (CURRENTLY INCOMPLETE)
public class ShieldSpell : Spell
{
    public override void cast(ICaster target, ICaster caster)
    {
        throw new System.NotImplementedException();
    }
}
//Contains the data associated with an Element keyword
public class ElementMod
{
    public string name;
    public string description;
    public int element;      //Elemental modifier to apply
    public float cooldownMod;
    public float cooldownModM;

}
//Contains the data associated with a Style keyword
public class StyleMod
{
    public string name;
    public string description;
    public int powerMod;
    public float powerModM = 0;
    public float cooldownMod;
    public float cooldownModM;
    public int accMod;
    public float accModM;
    public int critMod;
    public float critModM;
    public int statusEffectChanceMod;
    public float statusEffectChanceModM;
    public bool isTarget = true;
    public TargetData targets;
}
//Unfinished (CREATE A BETTER VERSION FOR TARGET MOD MODULATION)
public class TargetData
{
    public TargetData(bool b)
    {
        enemyL = b;
        enemyM = b;
        enemyR = b;
        allyL = b;
        allyM = b;
        allyR = b;
        selfCenter = b;
        targeted = b;

    }

    public bool enemyL;
    public bool enemyM;
    public bool enemyR;
    public bool allyL;
    public bool allyM;
    public bool allyR;
    public bool selfCenter;
    public bool targeted;

    public void modify(TargetData t)
    {
        enemyL = t.enemyL;
        enemyM = t.enemyM;
        enemyR = t.enemyR;
        if(!(allyL || allyM || allyR))
        {
            allyL = t.allyL;
            allyM = t.allyM;
            allyR = t.allyR;
            selfCenter = t.selfCenter;
        }
        targeted = t.targeted;
    }
}
