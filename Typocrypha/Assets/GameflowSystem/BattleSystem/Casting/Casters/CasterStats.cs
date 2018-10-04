using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TypocryphaGameflow.MathUtils;

[System.Serializable]
public class CasterStats
{
    public static readonly IntRange statRange = new IntRange(-10, 10);

    #region Resource Maxes
    public int maxHP;
    public int maxStagger;
    #endregion

    #region Stats
    [SerializeField] private float staggerTime;
    public float StaggerTime { get { return staggerTime; } set { staggerTime = value > 0 ? value : 0; } }
    [SerializeField] private int atk;
    public int Atk { get { return atk; } set { atk = statRange.clamp(value); } }
    [SerializeField] private int def;
    public int Def { get { return def; } set { def = statRange.clamp(value); } }
    [SerializeField] private int spd;
    public int Spd { get { return spd; } set { spd = statRange.clamp(value); } }
    [SerializeField] private int acc;
    public int Acc { get { return acc; } set { acc = statRange.clamp(value); } }
    [SerializeField] private int evade;
    public int Evade { get { return evade; } set { evade = statRange.clamp(value); } }
    #endregion
}
