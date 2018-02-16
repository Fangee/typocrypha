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

    public string[] animData;
    public string[] sfxData;

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

    //Just used in cast INNACURATE
    public bool reflect = false;

    //Used to set location data
    public void setLocationData(ICaster target, ICaster caster)
    {
        this.target = target;
        this.caster = caster;
    }

}
