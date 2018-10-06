using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Serializable dictionary wrapper (native Dictionary serialization not yet available)
// MUST BE INHERITED WITH TEMPLATE VARIABLES SET: generic classes are not serialized
[System.Serializable]
public class SerializableDictionary<Tkey, Tvalue>: ISerializationCallbackReceiver, IEnumerable
{
    private Dictionary<Tkey, Tvalue> _dictionary; // Internal dictionary interface
    [SerializeField] private List<Tkey> _keys; // Serlializable list of keys; Keys and values match up 1-to-1
    [SerializeField] private List<Tvalue> _values; // Serlializable list of values

    public SerializableDictionary()
    {
        _dictionary = new Dictionary<Tkey, Tvalue>();
        _keys = new List<Tkey>();
        _values = new List<Tvalue>();
    }

    #region Dictionary Implementation
    public int Count
    {
        get
        {
#if UNITY_EDITOR
            return Mathf.Min(_keys.Count, _values.Count);
#else
            return _dictionary.Count;
#endif
        }
    }
    public void Clear()
    {
#if UNITY_EDITOR
        _keys.Clear();
        _values.Clear();
#else
        _dictionary.Clear();
#endif
    }
    public bool ContainsKey(Tkey key)
    {
#if UNITY_EDITOR
        return _keys.Contains(key);
#else
        return _dictionary.ContainsKey(key);
#endif
    }
    public void Add(Tkey key, Tvalue value)
    {
#if UNITY_EDITOR
        _keys.Add(key);
        _values.Add(value);
#else
        _dictionary.Add(key, value);
#endif
    }
    public void Remove(Tkey key)
    {
#if UNITY_EDITOR
        _values.RemoveAt(_keys.IndexOf(key));
        _keys.Remove(key);
#else
        _dictionary.Remove(key);
#endif
    }
    public Tvalue this[Tkey key]
    {
        get
        {
#if UNITY_EDITOR
            return _values[_keys.IndexOf(key)];
#else
            return _dictionary[key];
#endif
        }
        set
        {
#if UNITY_EDITOR
            _values[_keys.IndexOf(key)] = value;
#else
            return _dictionary[key] = value;
#endif
        }
    }
    #endregion

    #region Editor Utilities
    public Tkey[] SortedKeys
    {
        get
        {
#if UNITY_EDITOR
            Tkey[] keys = _keys.ToArray();
            System.Array.Sort<Tkey>(keys);
            return keys;
#else
            Tkey[] keys = new Tkey[_dictionary.Count];
            _dictionary.Keys.CopyTo(keys, 0);
            System.Array.Sort<Tkey>(keys);
            return keys;
#endif
        }
    }
#if UNITY_EDITOR
    public void replaceKey(Tkey newVal, Tkey oldVal)
    {
        _keys[_keys.IndexOf(oldVal)] = newVal;
    }
#endif
    #endregion


    //iterates over keys
    IEnumerator IEnumerable.GetEnumerator()
    {
#if UNITY_EDITOR
        return _keys.GetEnumerator();
#else
        return _dictionary.Keys.GetEnumerator();
#endif
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
            catch (System.ArgumentException)
            {
                continue;
            }
        }
    }
    #endregion
}

