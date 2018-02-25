using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A class containing the result data from a casted spell (which can be used to generate animations an effects)
//Contains hit/miss, crit, stun, elemental weakness/resistance status, damage inflicted, etc.
//Does not contain cast status data (Botch, Fizzle)
public class CastData
{
    //Data fields
    public bool isHit = false;
    public bool isCrit = false;
    public bool isStun = false;
    public bool isBuff = false;
    public int damageInflicted = 0;
    public int element = Elements.notAnElement;
    public Elements.vsElement elementalData = Elements.vsElement.INVALID;
    public BuffData buffInflicted = null;

    //number of keywords in the spell
    public int wordCount;
    public string[] animData = new string[3];
    public string[] sfxData = new string[3];

    public ICaster Target
    {
        get { return target; }
    }
    public ICaster Caster
    {
        get { return caster; }
    }

    //Location data (used for targeting)
    ICaster target;
    ICaster caster;

    //Accurate now, but don't use this when you don't have to (may still be a little wonky)
    public bool repel = false;

    //Used to set location data
    public void setLocationData(ICaster target, ICaster caster)
    {
        this.target = target;
        this.caster = caster;
    }

}
