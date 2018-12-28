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

    #region Display Data
    public Sprite image;
    public bool overrideSpawnAnim = false;
    public AnimationClip spawnAnim = null;
    public bool overrideSpawnSfx = false;
    public AudioClip spawnSfx = null;
    public ATB2.Enemy.DeathAnimation deathAnim = ATB2.Enemy.DeathAnimation.Blastoff;
    #endregion

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