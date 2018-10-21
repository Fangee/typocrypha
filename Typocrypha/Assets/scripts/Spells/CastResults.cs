using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A class containing the result data from a casted spell effect
public class CastResults
{
    //The name of the root word this effect was attached to
    public string wordName;
    public bool isHit = false;
    public bool isCrit = false;
    public bool isStun = false;
    public int damageInflicted = 0;
    public ReactionType reaction = ReactionType.ANY;
    public SpellAnimationData[] animationData;
    public ICaster target;
    public ICaster caster;

    public SpellWord.SpellTagSet tags;

    //Accurate now, but don't use this when you don't have to (may still be a little wonky)
    public bool repel = false;

}
