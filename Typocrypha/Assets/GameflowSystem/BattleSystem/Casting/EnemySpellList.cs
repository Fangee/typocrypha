using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpellList
{
    public EnemySpellList(Dictionary<string, SpellData[]> spells)
    {
        groups = spells;
    }
    private Dictionary<string, SpellData[]> groups;
    public SpellData[] getSpells(string group = "DEFAULT")
    {
        return groups[group];
    }
    public bool hasGroup(string group)
    {
        return groups.ContainsKey(group);
    }
}
