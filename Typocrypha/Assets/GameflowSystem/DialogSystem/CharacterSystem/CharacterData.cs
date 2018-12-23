using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Container for data about specific characters (read only)
[CreateAssetMenu]
[System.Serializable]
public class CharacterData : ScriptableObject
{
    public NameSet aliases; // Different aliases/names for this character
    public NameMap poses; // Different body poses
    public NameMap expressions; // Different facial expressions
    public NameMap codecs; // Different codec sprites
    public Sprite chat_icon; // Chat mode sprite
    public UnityEngine.AudioClip talk_sfx; // Talking sound effect
}

// Serializable wrapper for dictionaries
[System.Serializable]
public class NameMap : SerializableDictionary<string, Sprite> { [System.NonSerialized] public string addField; }

// Serializable wrapper for sets
[System.Serializable]
public class NameSet : SerializableSet<string> { [System.NonSerialized] public string addField; }

