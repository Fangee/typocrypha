using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// displays enemies' charge bars for attacks
public class EnemyChargeBars : MonoBehaviour {
	public GameObject chargebar_prefab; // prefab for charge bar
	Enemy[] enemy_arr; // same enemy arr as in BattleManager
	BarMeter[] charge_bars; // charge bars for enemies

	// init enemy_arr and charge_bars arrays
	public void initChargeBars(Enemy[] i_arr) {
		enemy_arr = i_arr;
		charge_bars = new BarMeter[3];
	}

	// creates a new charge meter for an enemy at world_pos
	// pos is the enemy's position in the enemt array (0, 1, 2)
	public void makeChargeMeter(int pos, Vector3 world_pos) {
		GameObject new_bar = GameObject.Instantiate (chargebar_prefab, transform);
		new_bar.transform.localScale = new Vector3 (1, 1, 1);
		new_bar.transform.position = world_pos;
		charge_bars [pos] = new_bar.GetComponent<BarMeter> ();
		charge_bars [pos].setText ("");
	}

	// update charge bars
	void Update() {
		if (enemy_arr == null) return; // wait for initialization
		for (int i = 0; i < 3; i++) {
			if (enemy_arr [i] != null) {
				charge_bars [i].setValue (enemy_arr[i].getProgress());
			}
		}
	}
}
