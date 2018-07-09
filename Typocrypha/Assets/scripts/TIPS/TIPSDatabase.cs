using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Interfaces into TIPS database entries
public class TIPSDatabase : MonoBehaviour {
    public static TIPSDatabase main; // Global static reference

    void Awake()
    {
        if (main == null)
            main = this;
        else
            Destroy(this.gameObject);
    }

    // Queries database using a search term
    public List<TIPSEntry> Query(string search)
    {
        return null;
    }
}
