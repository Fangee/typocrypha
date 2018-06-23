using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class that holds the basic data for any spell (root keyword)
//A spell must inherit from this class to define specific functionality
public abstract class Spell
{
    //Enums

    public enum ModFlags { NORMAL, NO_ELEMENT, NO_STYLE, NO_TARGETING, NO_MODIFICATION}
    public enum WordType { ANY = -1, ROOT, ELEMENT, STYLE}

    //public methods

    //Casts this spell at selected target (at targets[selected])
    public abstract CastData cast(ICaster target, ICaster caster);
    //Starts spell cooldown using coroutine support from the Timer class 
    public void startCooldown(CooldownList l, string name, float time)
    {
        finish_time = time;
        l.add(name, time, curr_time, isNotOnCooldown);
    }
    //Apllies prefix and suffix to spell. both arguments can be null (if no prefix or suffix)
    public void Modify(ElementMod e, StyleMod s)
    {
        //Dont modify if unmodifyable
        if (modFlag == ModFlags.NO_MODIFICATION)
            return;
        //Add rest of stuff
        if (e != null && modFlag != ModFlags.NO_ELEMENT)//Add element modifier
        {
            element = e.element;
            name = e.name + "-" + name;
        }
        if (s != null && modFlag != ModFlags.NO_STYLE)//Add style modifier
        {
            //Modify targeting data if allowed and necessary
            if (s.isTarget == true)
            {
                if (modFlag == ModFlags.NO_TARGETING)
                    return;
                s.targets.modify(targetData);
            }
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
        if (targetData.enemyM)//Middle enemy
            castAt.Add(targets[i]);
        i--;
        if (i >= 0 && targetData.enemyL)//Left enemy
            castAt.Add(targets[i]);
        i += 2;
        if (i <= 2 && targetData.enemyR)//Right enemy
            castAt.Add(targets[i]);
        i = 1;
        if (targetData.selfCenter)//If spell is not cursor-dependant
            i += (position - 1);
        if (targetData.allyM)//Middle ally
            castAt.Add(allies[i]);
        i--;
        if (i >= 0 && targetData.allyL)//Left ally
            castAt.Add(allies[i]);
        i += 2;
        if (i <= 2 && targetData.allyR)//Right ally
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
        s.targetData.copyFrom(targetData);
        if(buff != null)
            s.buff = new BuffData(buff);
        s.buffPercentage = buffPercentage;
        s.modFlag = modFlag;
    }

    //protected methods

    //Return true if spell hits target, else false (does not actually apply spell effect)
    //Factors in target stunState if checkStun = true
    //ONLY CALL IN CAST (or after spell has been been properly modified with Modify())
    protected virtual bool hitCheck(CastData data, ICaster target, ICaster caster, bool checkStun = false)
    {
        if (checkStun && target.Is_stunned)
        {
            data.isHit = true;
            return true;
        }

        int chance = Mathf.CeilToInt(hitPercentage * caster.Stats.getModAcc(caster.BuffDebuff)) - target.Stats.getModEvade(target.BuffDebuff);
        if((Random.Range(0.0F, 1F) * 100) <= chance)
        {
            data.isHit = true;
            return true;
        }
        data.isHit = false;
        return false;
    }
    //Return true if spell crits target, else false. Multiplies power by 1.5 if a hit (round up)
    //Factors in target stunState if checkStun = true
    //ONLY CALL IN CAST (or after spell has been been properly modified with Modify())
    protected virtual bool critCheck(CastData data, ICaster target, ICaster caster, out int newPower)
    {
        float accBonus = Mathf.Clamp(((caster.Stats.getModAcc(caster.BuffDebuff) -1) * (1 - (target.Stats.getModEvade(target.BuffDebuff) * 0.01F))) * 0.2F, 0, 2) * 10;
        float chance = critPercentage + accBonus;
        if ((Random.Range(0.0F, 1F) * 100) <= chance)
        {
            data.isCrit = true;
            newPower = Mathf.CeilToInt(power * 1.5F);
            return true;
        }
        data.isCrit = false;
        newPower = power;
        return false;
    }
    //Returns true if spell inflicts a buff/debuff on target, and inflicts the buff/debuff
    protected virtual bool buffCheck(CastData data, ICaster target, ICaster caster, int powerMod)
    {
        if ((Random.Range(0.0F, 1F) * 100) <= buffPercentage)
        {
            data.isBuff = true;
            data.buffInflicted = buff;
            inflictBuff(target, caster, powerMod);
            return true;
        }
        return false;
    }
    //Inflects buff/debuff on target
    protected void inflictBuff(ICaster target, ICaster caster, float mod)
    {
        target.BuffDebuff.modify(buff.multiply(Mathf.CeilToInt(mod)));
    }

    //public fields

    public string name;
    public string description;          //Spell's description (in spellbook)
    public string animationID;          //Spell's animation ID 
    public string sfxID;                //Spell's sfx ID
    public int power;                   //Spell's intensity (not necessarily just damage)
    public float cooldown;              //Spell's base cooldown
    public int hitPercentage;           //Spell's base hit chance (1 = 1%)
    public int critPercentage;          //Spell's base crit chance (1 = 1%)
    public int elementEffectMod;        //Spell's base elemental effect chance (1 = 1%)
    public int element = Elements.@null;     //Spell's elemental damage type
    public BuffData buff = null;        //Spell's buff/debuff to inflict
    public int buffPercentage;          //Chance of inflicting buff/debuff
    public string type = "null";        //Spell's effect type (attack, shield, heal, etc.)
    public TargetData targetData;       //Spell's targeting data
    public ModFlags modFlag;            //Shows how to modify this spell

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
//Spells that attempt to do damage to opposing entities (or self and/or allies)
//Can have a chance to buff/debuff
public class AttackSpell : Spell
{
    public override CastData cast(ICaster target, ICaster caster)
    {
        CastData data = new CastData();
        data.element = element;
        if(hitCheck(data, target,caster, true))
        {
            int powerMod;
            bool crit = critCheck(data, target, caster, out powerMod);                
            target.damage(data, powerMod, element, caster, crit);
            if (buff != null)
                buffCheck(data, target, caster, powerMod);
        }
        return data;
    }


}
//Spells that buff/debuff enemies, but do not do damage
public class BuffSpell : Spell
{
    public override CastData cast(ICaster target, ICaster caster)
    {
        CastData data = new CastData();
        data.element = element;
        int powerMod;
        critCheck(data, target, caster, out powerMod);
        buffCheck(data, target, caster, powerMod);
        return data;
    }
    protected override bool buffCheck(CastData data, ICaster target, ICaster caster, int powerMod)
    {
        Elements.vsElement vs = target.Stats.getModVsElement(target.BuffDebuff, element);
        switch (vs)
        {
            case Elements.vsElement.REPEL:
                data.repel = true;
                inflictBuff(caster, target, powerMod);
                break;
            case Elements.vsElement.DRAIN:
                buff.makeBuff();
                inflictBuff(target, caster, powerMod);
                break;
            case Elements.vsElement.RESIST:
                if ((Random.Range(0.0F, 1F) * 100) > 50)
                {
                    data.isHit = false;
                    return false;
                }
                inflictBuff(target, caster, powerMod);
                break;
            case Elements.vsElement.BLOCK:
                data.isHit = false;
                return false;
            case Elements.vsElement.NEUTRAL:
                inflictBuff(target, caster, powerMod);
                break;
            case Elements.vsElement.WEAK:
                inflictBuff(target, caster, powerMod * 1.5F);
                break;
            case Elements.vsElement.SUPERWEAK:
                inflictBuff(target, caster, powerMod * 2);
                break;
            case Elements.vsElement.INVALID:
                throw new System.NotImplementedException();
        }
        data.isBuff = true;
        data.isHit = true;
        data.buffInflicted = buff;
        return true;
    }
}

//Spells that attempt to heal friendly entities (CURRENTLY INCOMPLETE)
public class HealSpell : Spell
{
    public override CastData cast(ICaster target, ICaster caster)
    {
        throw new System.NotImplementedException();
    }
}
//Spells that attempt to shield friendly entities (CURRENTLY INCOMPLETE)
public class ShieldSpell : Spell
{
    public override CastData cast(ICaster target, ICaster caster)
    {
        throw new System.NotImplementedException();
    }
}
//Contains the data associated with an Element keyword
public class ElementMod
{
    public string name;
    public string description;
    public string animationID;        
    public string sfxID;               
    public int element;      //Elemental modifier to apply
    public float cooldownMod;
    public float cooldownModM;

}
//Contains the data associated with a Style keyword
public class StyleMod
{
    public string name;
    public string description;
    public string animationID;
    public string sfxID;
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
    public TargetMod targets;
}
//Contains targeting data and associated targeting modification methods
public class TargetData
{
    public bool enemyL;
    public bool enemyM;
    public bool enemyR;
    public bool allyL;
    public bool allyM;
    public bool allyR;
    public bool selfCenter;
    public bool targeted;
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
    public TargetData(string pattern)
    {
        if (pattern.Contains("A"))
        {
            enemyL = true;
            enemyM = true;
            enemyR = true;
            allyL = true;
            allyM = true;
            allyR = true;
            selfCenter = false;
            targeted = false;
        }
        else
        {
            if (pattern.Contains("L"))
                enemyL = true;
            if (pattern.Contains("M"))
                enemyM = true;
            if (pattern.Contains("R"))
                enemyR = true;
            if (pattern.Contains("l"))
                allyL = true;
            if (pattern.Contains("m"))
                allyM = true;
            if (pattern.Contains("r"))
                allyR = true;
            if (pattern.Contains("S"))
                selfCenter = true;
            if (pattern.Contains("T"))
                targeted = true;
        }
    }
    public Pair<bool[], bool[]> toArrayPair(ICaster[] targets, int selected, ICaster[] allies, int position)
    {
        bool[] enemy_r = { false, false, false };
        bool[] ally_r = { false, false, false };
        int i = 1;
        if (targeted)//If spell is cursor-dependant
            i += (selected - 1);
        if (enemyM && targets[i] != null && !targets[i].Is_dead)//Middle enemy
            enemy_r[i] = true;
        i--;
        if (i >= 0 && enemyL && targets[i] != null && !targets[i].Is_dead)//Left enemy
            enemy_r[i] = true;
        i += 2;
        if (i <= 2 && enemyR && targets[i] != null && !targets[i].Is_dead)//Right enemy
            enemy_r[i] = true;
        i = 1;
        if (selfCenter)//If spell is not cursor-dependant
            i += (position - 1);
        if (allyM && allies[i] != null && !allies[i].Is_dead)//Middle ally
            ally_r[i] = true;
        i--;
        if (i >= 0 && allyL && allies[i] != null && !allies[i].Is_dead)//Left ally
            ally_r[i] = true;
        i += 2;
        if (i <= 2 && allyR && allies[i] != null && !allies[i].Is_dead)//Right ally
            ally_r[i] = true;
        return new Pair<bool[], bool[]>(enemy_r, ally_r);
    }
    //Modifies this by target data mod
    public void modify(TargetData mod)
    {
        bool targets_enemy = (enemyL || enemyM || enemyR);
        bool targets_ally = (allyL || allyM || allyR);
        bool mod_targets_enemy = (mod.enemyL || mod.enemyM || mod.enemyR);
        bool mod_targets_ally = (mod.allyL || mod.allyM || mod.allyR);
        if (targets_enemy && targets_ally && mod_targets_ally && mod_targets_enemy)
        {
            enemyL = mod.enemyL;
            enemyM = mod.enemyM;
            enemyR = mod.enemyR;
            allyL = mod.allyL;
            allyM = mod.allyM;
            allyR = mod.allyR;
            targeted = mod.targeted;
            selfCenter = mod.selfCenter;
        }
        else if (targets_enemy)
        {
            enemyL = mod.enemyL;
            enemyM = mod.enemyM;
            enemyR = mod.enemyR;
            targeted = mod.targeted;
        }
        else if (targets_ally)
        {
            allyL = mod.allyL;
            allyM = mod.allyM;
            allyR = mod.allyR;
            selfCenter = mod.selfCenter;
        }
    }
    public void copyFrom(TargetData toCopy)
    {
        enemyL = toCopy.enemyL;
        enemyM = toCopy.enemyM;
        enemyR = toCopy.enemyR;
        targeted = toCopy.targeted;
        allyL = toCopy.allyL;
        allyM = toCopy.allyM;
        allyR = toCopy.allyR;
        selfCenter = toCopy.selfCenter;
    }
}
