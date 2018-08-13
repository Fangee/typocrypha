using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Serializable HashSet wrapper (native HashSet serialization not yet available)
// Should only be set in Editor more; Is read only in game mode
// MUST BE INHERITED WITH TEMPLATE VARIABLES SET: generic classes are not serialized
[System.Serializable]
public class SerializableSet<T> : ISerializationCallbackReceiver, IEnumerable
{
    HashSet<T> _hashset; // Internal HashSet interface
    public List<T> _items; // Serializable list of items

    public SerializableSet()
    {
        _hashset = new HashSet<T>();
        _items = new List<T>();
    }

    public bool Contains(T item)
    {
        return _hashset.Contains(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _hashset.GetEnumerator();
    }

    #region serialization
    public void OnBeforeSerialize() { }

    // Convert list back into set
    public void OnAfterDeserialize()
    {
        _hashset = new HashSet<T>();

        foreach (T item in _items)
        {
            _hashset.Add(item);
        }
    }
    #endregion
}
