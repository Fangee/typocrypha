using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu] // attribute allows creation of EnemyData assets in unity editor
public class EnemyData : ScriptableObject
{
	// Enemy sprite
	public Image image;

    public Casting.CasterStats stats;
		
	// AI settings
	public string AIType;
	public string[] AIParameters;
	
	// Spells
	public SpellMap spells;
	
	// Serializable SpellMap Dictionary
	[System.Serializable]
    public class SpellMap : SerializableDictionary<string, SpellData[]> {}

}
