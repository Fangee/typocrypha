using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Serializable dictionary wrapper (native Dictionary serialization not yet available)
// Should only be set in Editor mode; Is read-only in game mode
// MUST BE INHERITED WITH TEMPLATE VARIABLES SET: generic classes are not serialized
[System.Serializable]
public class SerializableDictionary<Tkey, Tvalue> : ISerializationCallbackReceiver, IEnumerable
{
    Dictionary<Tkey, Tvalue> _dictionary; // Internal dictionary interface
    public List<Tkey> _keys; // Serlializable list of keys; Keys and values match up 1-to-1
    public List<Tvalue> _values; // Serlializable list of values

    public SerializableDictionary()
    {
        _dictionary = new Dictionary<Tkey, Tvalue>();
        _keys = new List<Tkey>();
        _values = new List<Tvalue>();
    }

    public Tvalue this[Tkey key]
    {
        get
        {
            return _dictionary[key];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _dictionary.GetEnumerator();
    }

    #region serialization
    public void OnBeforeSerialize() { }

    // Convert lists back into dictionary
    public void OnAfterDeserialize()
    {
        _dictionary = new Dictionary<Tkey, Tvalue>();

        for (int i = 0; i != System.Math.Min(_keys.Count, _values.Count); ++i)
        {
            try
            {
                _dictionary.Add(_keys[i], _values[i]);
            }
            catch (System.ArgumentException e)
            {
                continue;
            }
        }
    }
    #endregion
}
