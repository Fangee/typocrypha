using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// manages battle sequences
public class BattleManager : MonoBehaviour {
	public static BattleManager main = null; // static instance accessible globally
	public GameObject spellDict; // spell dictionary object
	public GameObject enemy_prefab; // prefab for enemy object
	public EnemyChargeBars charge_bars; // creates and amanges charge bars
	public int target_ind; // index of currently targeted enemy
	public Transform target_ret; // shows where target is
	public float enemy_spacing; // space between enemies
	public bool pause; // is battle paused?
	public Enemy[] enemy_arr; // array of Enemy components (size 3)

	void Awake() {
		if (main == null) main = this;
		pause = false;
	}

	// start battle scene
	public void startBattle(BattleScene scene) {
		Debug.Log ("Battle! (goes on infinitely)");
		enemy_arr = new Enemy[3];
		charge_bars.initChargeBars ();
		for (int i = 0; i < scene.enemy_stats.Length; i++) {
			GameObject new_enemy = GameObject.Instantiate (enemy_prefab, transform);
			new_enemy.transform.localScale = new Vector3 (1, 1, 1);
			new_enemy.transform.localPosition = new Vector3 (i * enemy_spacing, 0, 0);
			enemy_arr [i] = new_enemy.GetComponent<Enemy> ();
			enemy_arr [i].setStats (scene.enemy_stats [i]);
            enemy_arr [i].field = enemy_arr; //Give enemey access to field
            enemy_arr [i].position = i;      //Log enemy position in field
			Vector3 bar_pos = new_enemy.transform.position;
			bar_pos.Set (bar_pos.x, bar_pos.y + 1, bar_pos.z);
			charge_bars.makeChargeMeter(i, bar_pos);
		}
		target_ind = 0;
	}

	// check if player switches targets or attacks
	void Update() {
		// move target left or right
		if (Input.GetKeyDown (KeyCode.LeftArrow)) --target_ind;
		if (Input.GetKeyDown (KeyCode.RightArrow)) ++target_ind;
		// fix if target is out of bounds
		if (target_ind < 0) target_ind = 0;
		if (target_ind > 2) target_ind = 2;
		// move target reticule
		target_ret.localPosition = new Vector3 (target_ind * enemy_spacing, -1, 0);
	}

	// attack currently targeted enemy with spell
	public void attackCurrent(string spell) {
		if (enemy_arr [target_ind].is_dead) {
			Debug.Log ("target is alrady dead!");
			return; // don't attack dead enemies
		}
		StartCoroutine (pauseAttackCurrent (spell));
    }

	// pause for player attack, play animations, unpause
	IEnumerator pauseAttackCurrent(string spell){
		BattleManager.main.pause = true;
		BattleEffects.main.setDim (true, enemy_arr[target_ind].enemy_sprite);
		yield return new WaitForSeconds (1f);
        //	BattleEffects.main.spriteShake (enemy_arr[target_ind].gameObject.transform, 0.5f, 0.1f);
        //Send spell, Enemy state, and target index to parser and caster
        spellDict.GetComponent<SpellDictionary>().parseAndCast(spell.ToLower(), enemy_arr, target_ind, Player.main);
        yield return new WaitForSeconds (1f);
		BattleEffects.main.setDim (false, enemy_arr [target_ind].enemy_sprite);
        updateEnemies();
		BattleManager.main.pause = false;
	}
    //Updates death and opacity of enemies after pause in puaseAttackCurrent
    private void updateEnemies()
    {
        for(int i = 0; i < enemy_arr.Length; i++)
        {
            if (!enemy_arr[i].is_dead)
                enemy_arr[i].updateCondition();
        }
    }
}
