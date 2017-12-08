using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// manages battle sequences
public class BattleManager : MonoBehaviour {
	public StateManager state_manager; // manages global state/scenes
	public GameObject spellDict; // spell dictionary object
	public GameObject enemy_prefab; // prefab for enemy object
	public int target_ind; // index of currently targeted enemy
	public Transform target_ret; // shows where target is
	public float enemy_spacing; // space between enemies
	Enemy[] enemy_arr; // array of Enemy components (size 3)

	void Start() {

	}

	// start battle scene
	public void startBattle(BattleScene scene) {
		Debug.Log ("Battle! (goes on infinitely)");
		enemy_arr = new Enemy[3];
		for (int i = 0; i < scene.enemy_stats.Length; i++) {
			GameObject new_enemy = GameObject.Instantiate (enemy_prefab, transform);
			new_enemy.transform.localScale = new Vector3 (1, 1, 1);
			new_enemy.transform.localPosition = new Vector3 (i * enemy_spacing, 0, 0);
			enemy_arr [i] = new_enemy.GetComponent<Enemy> ();
			enemy_arr [i].setStats (scene.enemy_stats [i]);
            enemy_arr[i].field = enemy_arr; //Give enemey access to field
            enemy_arr[i].position = i;      //Log enemy position in field
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
        //Send spell, Enemy state, and traget index to parser and caster 
        spellDict.GetComponent<SpellDictionary>().parseAndCast(spell, enemy_arr, target_ind,Player.main);
    }
}
