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
    public int maxHP;
    public int maxMP;
    public int maxStagger;
    public float atk;
    public float def;
    public float spd;
    public float acc;
    public float evade;

    // Type Weaknesses/Resistances
    public float vsNull;
    public float vsFire;
    public float vsIce;
    public float vsVolt;

    // AI settings
    public string AIType;
    public string[] AIParameters;

    // Spells
    //public SpellMap spells;

    //// Serializable SpellMap Dictionary
    //[System.Serializable]
    //public class SpellMap : SerializableDictionary<string, SpellData[]> { }

}
