using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleDataTracker {

    [HideInInspector] public System.DateTime time_started; // time battle started
    [HideInInspector] public Battlefield.FieldPosition lastCaster = Battlefield.FieldPosition.NONE;
    [HideInInspector] public List<CastData> last_enemy_cast; // last performed cast action
    [HideInInspector] public List<CastData> last_player_cast;
    [HideInInspector] public SpellData last_enemy_spell; // last performed spell
    [HideInInspector] public SpellData last_player_spell; // last performed spell
    [HideInInspector] public bool[] last_register; // last spell register status
    [HideInInspector] public int num_player_attacks; // number of player attacks from beginning of battle
    public Dictionary<string, string> flags = new Dictionary<string, string>();
}
