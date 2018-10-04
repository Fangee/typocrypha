using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu] // attribute allows creation of EnemyData assets in unity editor
public class AllyData : ScriptableObject
{
    // Enemy sprite
    public Sprite BattleIcon;
    public Sprite sprite;

    // Stats
    public CasterStats stats;

    // AI settings (Replace w/ SO)
    public string AIType;
    public string[] AIParameters;

    // Spells
    //public SpellMap spells;

    //// Serializable SpellMap Dictionary
    //[System.Serializable]
    //public class SpellMap : SerializableDictionary<string, SpellData[]> { }

}
