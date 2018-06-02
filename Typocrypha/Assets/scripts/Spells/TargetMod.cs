using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetMod
{

    public abstract void modify(TargetData t);
    public static TargetMod createFromString(string key, string[] parameters)
    {
        switch (key)
        {
            case "relative":
                return new TargetModRelative(parameters);
            case "r":
                return new TargetModRelative(parameters);
            case "absolute":
                return new TargetModAbsolute(parameters);
            case "a":
                return new TargetModAbsolute(parameters);
            case "invert":
                return new TargetModInvert();
            case "swap":
                return new TargetModInvert();
            case "copy":
                return new TargetModCopy();
            case "self":
                return new TargetModSelf();
            case "other":
                return new TargetModOther();
        }
        throw new System.NotImplementedException(key + " is not a TargetMod type!");
    }
}
public class TargetModAbsolute : TargetMod
{
    TargetData mod;
    public TargetModAbsolute(string[] parameters)
    {
        mod = new TargetData(parameters[1]);
    }
    public override void modify(TargetData t)
    {
        t.enemyL = mod.enemyL;
        t.enemyM = mod.enemyM;
        t.enemyR = mod.enemyR;
        t.allyL = mod.allyL;
        t.allyM = mod.allyM;
        t.allyR = mod.allyR;
        t.targeted = mod.targeted;
        t.selfCenter = mod.selfCenter;
    }
}
public class TargetModRelative : TargetMod
{
    TargetData mod;
    public TargetModRelative(string[] parameters)
    {
        mod = new TargetData(parameters[1]);
    }
    public override void modify(TargetData t)
    {
        bool targets_enemy = (t.enemyL || t.enemyM || t.enemyR);
        bool targets_ally = (t.allyL || t.allyM || t.allyR);
        if (targets_enemy)
        {
            t.enemyL = mod.enemyL;
            t.enemyM = mod.enemyM;
            t.enemyR = mod.enemyR;
        }
        if (targets_ally)
        {
            t.allyL = mod.allyL;
            t.allyM = mod.allyM;
            t.allyR = mod.allyR;
        }
    }
}
public class TargetModCopy : TargetMod
{
    public override void modify(TargetData t)
    {
        bool targets_enemy = (t.enemyL || t.enemyM || t.enemyR);
        bool targets_ally = (t.allyL || t.allyM || t.allyR);
        if (targets_enemy && targets_ally)
            return;
        else if (targets_enemy)
        {
            t.allyL = t.enemyL;
            t.allyM = t.enemyM;
            t.allyR = t.enemyR;
        }
        else if (targets_ally)
        {
            t.enemyL = t.allyL;
            t.enemyM = t.allyM;
            t.enemyR = t.allyR;
        }
    }
}
public class TargetModInvert : TargetMod
{
    public override void modify(TargetData t)
    {
        t.enemyL = !t.enemyL;
        t.enemyM = !t.enemyM;
        t.enemyR = !t.enemyR;
        t.allyL = !t.allyL;
        t.allyM = !t.allyM;
        t.allyR = !t.allyR;
    }
}
public class TargetModSwap : TargetMod
{
    public override void modify(TargetData t)
    {
        bool tempL = t.enemyL;
        bool tempM = t.enemyM;
        bool tempR = t.enemyR;
        t.enemyL = t.allyL;
        t.enemyM = t.allyM;
        t.enemyR = t.allyR;
        t.allyL = tempL;
        t.allyM = tempM;
        t.allyR = tempR;
    }
}
public class TargetModSelf : TargetMod
{
    public override void modify(TargetData t)
    {
        bool targets_enemy = (t.enemyL || t.enemyM || t.enemyR);
        if(targets_enemy)
        {
            t.allyL = t.enemyL;
            t.allyM = t.enemyM;
            t.allyR = t.enemyR;
            t.enemyL = false;
            t.enemyM = false;
            t.enemyR = false;
        }
    }
}
public class TargetModOther : TargetMod
{
    public override void modify(TargetData t)
    {
        bool targets_ally = (t.allyL || t.allyM || t.allyR);
        if(targets_ally)
        {
            t.enemyL = t.allyL;
            t.enemyM = t.allyM;
            t.enemyR = t.allyR;
            t.allyL = false;
            t.allyM = false;
            t.allyR = false;
            t.targeted = true;
        }
    }
}

