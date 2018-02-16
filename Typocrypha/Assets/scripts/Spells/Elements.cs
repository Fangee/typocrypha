using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class containing element constants and associated methods.
//Essentially a glorified int enum
public static class Elements
{
    //Elemental vs status
    public enum vsElement
    {
        INVALID,
        REPEL,
        DRAIN,
        BLOCK,
        RESIST,
        NEUTRAL,
        WEAK,
        SUPERWEAK,
    }
    //Total number of elements (including null) in the game
    public const int count = 4;
    //Element constants (integers due to being frequently used as array indices)
    public const int notAnElement = -1;
    public const int @null = 0;
    public const int fire = 1;
    public const int ice = 2;
    public const int volt = 3;
    //Constants used in calcdamage, don't use elsewhere
    public const int drain = -1;
    public const int repel = -2;

    //Returns integer form of element for equivalent elementName string
    public static int fromString(string elementName)
    {
        switch (elementName)
        {
            case "null":
                return @null;
            case "fire":
                return fire;
            case "ice":
                return ice;
            case "volt":
                return volt;
            default:
                return notAnElement;
        }
    }
    //Returns string form of integer elements
    public static string toString(int elementNum)
    {
        switch (elementNum)
        {
            case 0:
                return "null";
            case 1:
                return "fire";
            case 2:
                return "ice";
            case 3:
                return "volt";
            default:
                return "not an element";
        }
    }

    //Buff/Debuff code (complicated)

    public static vsElement modLevel(vsElement level, int amount)
    {
        if (level != vsElement.INVALID)
            return (vsElement)(Utility.Math.Clamp((int)level + amount, 1, 7));
        return vsElement.INVALID;
    }
    public static vsElement getLevel(float value)
    {
        if (value == -2F)
            return vsElement.REPEL;
        else if (value == -1F)
            return vsElement.DRAIN;
        else if (value == 1F)
            return vsElement.NEUTRAL;
        else if (value == 0F)
            return vsElement.BLOCK;
        else if (value < 1)
            return vsElement.RESIST;
        else if (value > 2)
            return vsElement.SUPERWEAK;
        else if (value > 1)
            return vsElement.WEAK;
        else
            return vsElement.INVALID;
    }
    public static float getFloat(vsElement level)
    {
        switch (level)
        {
            case vsElement.NEUTRAL:
                return 1;
            case vsElement.BLOCK:
                return 0;
            case vsElement.REPEL:
                return repel;
            case vsElement.DRAIN:
                return drain;
            case vsElement.RESIST:
                return 0.5F;
            case vsElement.WEAK:
                return 1.5F;
            case vsElement.SUPERWEAK:
                return 2;
            case vsElement.INVALID:
                throw new System.NotImplementedException();
            default:
                throw new System.NotImplementedException();
        }
    }

}
