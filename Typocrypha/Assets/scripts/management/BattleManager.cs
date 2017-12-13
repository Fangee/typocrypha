using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// manages battle sequences
public class BattleManager : MonoBehaviour {
	public static BattleManager main = null; // static instance accessible globally
	public GameObject spellDict; // spell dictionary object
	public GameObject enemy_prefab; // prefab for enemy object
	public EnemyChargeBars charge_bars; // creates and mananges charge bars
	public CooldownList cooldown_list; // creates and manages player's cooldowns
	public Transform target_ret; // shows where target is
	public float enemy_spacing; // space between enemies
	public bool pause; // is battle paused?
	public Enemy[] enemy_arr; // array of Enemy components (size 3)
    public int target_ind; // index of currently targeted enemy
    public ICaster[] player_arr = { null, Player.main, null }; // array of Player and allies (size 3)
    public int player_ind = 1;
	public int enemy_count; // number of enemies in battle

	void Awake() {
		if (main == null) main = this;
		pause = false;
	}

	// start battle scene
	public void startBattle(BattleScene scene) {
		Debug.Log ("Battle!");
		enemy_arr = new Enemy[3];
		enemy_count = scene.enemy_stats.Length;
		charge_bars.initChargeBars ();
		for (int i = 0; i < scene.enemy_stats.Length; i++) {
			GameObject new_enemy = GameObject.Instantiate (enemy_prefab, transform);
			new_enemy.transform.localScale = new Vector3 (1, 1, 1);
			new_enemy.transform.localPosition = new Vector3 (i * enemy_spacing, 0, 0);
			enemy_arr [i] = new_enemy.GetComponent<Enemy> ();
			enemy_arr [i].setStats (scene.enemy_stats [i]);
            enemy_arr [i].allies = enemy_arr; //Give enemey access to field
            enemy_arr [i].position = i;      //Log enemy position in field
            enemy_arr[i].targets = player_arr;//Give enemy access to player field
            enemy_arr[i].bars = charge_bars; //Give enemy access to charge_bars
			Vector3 bar_pos = new_enemy.transform.position;
			bar_pos.Set (bar_pos.x, bar_pos.y + 1, bar_pos.z);
			charge_bars.makeChargeMeter(i, bar_pos);
		}
		pause = false;
		target_ind = 0;
		AudioPlayer.main.playMusic (MusicType.BATTLE, scene.music_tracks[0]);
	}

	// check if player switches targets or attacks
	void Update() {
		int old_ind = target_ind;
		// move target left or right
		if (Input.GetKeyDown (KeyCode.LeftArrow)) --target_ind;
		if (Input.GetKeyDown (KeyCode.RightArrow)) ++target_ind;
		// fix if target is out of bounds
		if (target_ind < 0) target_ind = 0;
		if (target_ind > 2) target_ind = 2;
		// move target reticule
		target_ret.localPosition = new Vector3 (target_ind * enemy_spacing, -1, 0);
		// play effect sound if target was moved
		if (old_ind != target_ind) AudioPlayer.main.playSFX(0, SFXType.UI, "sfx_enemy_select");
	}

	// attack currently targeted enemy with spell
	public void attackCurrent(string spell) {
		if (enemy_arr [target_ind].is_dead) {
			Debug.Log ("target is already dead!");
			return; // don't attack dead enemies
		}
		StartCoroutine (pauseAttackCurrent (spell));
    }

	// pause for player attack, play animations, unpause
	IEnumerator pauseAttackCurrent(string spell){
		pause = true;
		BattleEffects.main.setDim (true, enemy_arr[target_ind].enemy_sprite);
		//AudioPlayer.main.playSFX (1, SFXType.SPELL, "magic_sound");
		yield return new WaitForSeconds (1.5f);
		AudioPlayer.main.playSFX (1, SFXType.SPELL, "Cutting_SFX");
		AnimationPlayer.main.playAnimation (AnimationType.SPELL, "cut", enemy_arr[target_ind].transform.position, false);
		//Send spell, Enemy state, and target index to parser and caster
        spellDict.GetComponent<SpellDictionary>().parseAndCast(spell.ToLower(), enemy_arr, target_ind, player_arr, player_ind);
        yield return new WaitForSeconds (1f);
		BattleEffects.main.setDim (false, enemy_arr [target_ind].enemy_sprite);
        updateEnemies();
		pause = false;
	}
    //Updates death and opacity of enemies after pause in puaseAttackCurrent
    private void updateEnemies()
    {
		int curr_dead = 0;
        for(int i = 0; i < enemy_arr.Length; i++)
        {
			enemy_arr [i].updateCondition ();
			if (enemy_arr [i].is_dead) ++curr_dead;
        }
		if (curr_dead == enemy_count) // end battle if all enemies dead
		{
			Debug.Log("you win!");
			cooldown_list.removeAll ();
			StartCoroutine(StateManager.main.nextSceneDelayed(2.0f));
		}
    }
	//removes all enemies and charge bars
	public void stopBattle() {
		pause = true;
		foreach (Enemy enemy in enemy_arr) {
			if (enemy != null) GameObject.Destroy (enemy.gameObject);
		}
		enemy_arr = null;
		cooldown_list.removeAll ();
		charge_bars.removeAll ();
	}
}
