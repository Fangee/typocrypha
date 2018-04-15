using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Encapsulates battle info
public class BattleField {
	[HideInInspector] public Enemy[] enemy_arr; // array of Enemy components (size 3)
	[HideInInspector] public int target_ind; // index of currently targeted enemy
	[HideInInspector] public ICaster[] player_arr = { null, null, null }; // array of Player and allies (size 3)
	[HideInInspector] public int player_ind = 1;
	[HideInInspector] public int enemy_count; // number of enemies in battle
	[HideInInspector] public int curr_dead;
	[HideInInspector] public Vector2 target_pos; // position of target ret
	[HideInInspector] public System.DateTime time_started; // time battle started
	[HideInInspector] public List<CastData> last_cast; // last performed cast action
	[HideInInspector] public SpellData last_spell; // last performed spell
	[HideInInspector] public bool[] last_register; // last spell register status
	[HideInInspector] public int num_player_attacks; // number of player attacks from beginning of battle
}
