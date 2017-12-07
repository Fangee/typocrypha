using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// simple container for enemy stats
public struct EnemyStats {
	public string name;    // name of enemy
	public int max_hp;     // max health
	public float atk_time; // time it takes to attack
	// also will eventually have other stats
}

// defines enemy behaviour
public class Enemy : MonoBehaviour {
	public bool is_dead; // is enemy dead?

	EnemyStats stats; // stats of enemy
	int curr_hp; // current amount of health
	float curr_time; // current time (from 0 to atk_time)

	void Start() {
		is_dead = false;
	}

	public void setStats(EnemyStats i_stats) {
		stats = i_stats;
		curr_hp = stats.max_hp;
		curr_time = 0;
		StartCoroutine (timer ());
	}

	// keep track of time, and attack whenever curr_time = atk_time
	IEnumerator timer() {
		while (!is_dead) {
			yield return new WaitForSeconds (0.1f);
			curr_time += 0.1f;
			if (curr_time >= stats.atk_time) {
				attackPlayer ("some enemy spell");
				curr_time = 0;
			}
		}
	}

	// attacks player with specified spell
	// NOTE: once we have a spell class/etc, we might want to replace parameter with that
	void attackPlayer(string spell) {
		Debug.Log (stats.name + " casts " + spell);
	}

	// be attacked by the player
	// NOTE: once we have a spell class/etc, we might want to replace parameter with that
	public void beAttacked(string spell) {
		Debug.Log (stats.name + " was hit by " + spell);
		curr_hp -= 10; // TEMP: hard coded for now
		if (curr_hp <= 0) { // check if killed
			Debug.Log (stats.name + " has been slain!");
			is_dead = true;
			GameObject.Destroy (gameObject);
		}
	}
}
