using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIntel {

    public static EnemyIntel main = new EnemyIntel();

    private Dictionary<string, bool[]> intel = new Dictionary<string, bool[]>();

    //learn what effect an element has on an enemy
    public void learnIntel(string enemy, int element) {
        if (!intel.ContainsKey(enemy))
        {
            intel.Add(enemy, new bool[Elements.count]);
        }
        intel[enemy][element] = true;
    }

    public bool hasIntel(string enemy, int element) {
        if (intel.ContainsKey(enemy)) {
            return intel[enemy][element];
        }
        return false;
    }

}
