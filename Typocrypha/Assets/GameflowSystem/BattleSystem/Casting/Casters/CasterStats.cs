using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TypocryphaGameflow.MathUtils;

[System.Serializable]
public class CasterStats
{
    public static readonly IntRange statRange = new IntRange(0, 100);

    #region Resource Maxes
    public int maxHP;
    public int maxStagger;
    #endregion

    #region Stats
    private int atk;
    public int Atk { get { return atk; } set { atk = statRange.clamp(value); } }
    private int def;
    public int Def { get { return def; } set { def = statRange.clamp(value); } }
    private int spd;
    public int Spd { get { return spd; } set { spd = statRange.clamp(value); } }
    private int acc;
    public int Acc { get { return acc; } set { acc = statRange.clamp(value); } }
    private int evade;
    public int Evade { get { return evade; } set { evade = statRange.clamp(value); } }
    #endregion
}
