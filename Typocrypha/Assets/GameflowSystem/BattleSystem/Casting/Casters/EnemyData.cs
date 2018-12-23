using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CreateAssetMenu] // attribute allows creation of EnemyData assets in unity editor
public class EnemyData : ScriptableObject
{
    //Display Name
    public string displayName = string.Empty;
	// Enemy sprite
	public Sprite image;
    
    public CasterStats stats = new CasterStats();
		
	// AI settings
	public string AIType;
	public string[] AIParameters;
	
	// Spells
	public SpellMap spells;

    // Tags
    public CasterTagDictionary tags = new CasterTagDictionary();
    //public CasterTagDictionary tags;

    // Serializable SpellMap Dictionary
    [System.Serializable] public class SpellMap : SerializableDictionary<string, SpellData[]> {}
}