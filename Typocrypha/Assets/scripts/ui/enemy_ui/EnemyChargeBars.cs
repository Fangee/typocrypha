using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// displays enemies' charge bars for attacks
public class EnemyChargeBars : MonoBehaviour {
	public GameObject chargebar_prefab; // prefab for charge bar
	public float bar_width; // width of bar
	Vector3 x_offset; // offset to account for width of bar
	BarMeter[] charge_bars; // charge bars for enemies

    //Get-only
    public BarMeter[] Charge_bars
    {
        get
        {
            return charge_bars;
        }
    }

    void Awake() {
		x_offset = new Vector3 (-0.5f * bar_width, 0, 0);
		charge_bars = new BarMeter[3];
	}

	// init charge bars
	public void initChargeBars() {
		if (charge_bars != null)
			foreach (BarMeter bar in charge_bars)
				if (bar != null) GameObject.Destroy(bar.gameObject);
		charge_bars = new BarMeter[3];
		this.enabled = true;
	}

	// creates a new charge meter for an enemy at world_pos
	// pos is the enemy's position in the enemt array (0, 1, 2)
	public void makeChargeMeter(int pos, Vector3 world_pos) {
		GameObject new_bar = GameObject.Instantiate (chargebar_prefab, transform);
		new_bar.transform.localScale = new Vector3 (1, 1, 1);
		new_bar.transform.position = world_pos;
		new_bar.transform.localPosition += x_offset;
		charge_bars [pos] = new_bar.GetComponent<BarMeter> ();
		charge_bars [pos].setText ("");
	}

	// removes all charge bars
	public void removeAll() {
		this.enabled = false;
		foreach (BarMeter chargebar in charge_bars)
			if (chargebar != null) GameObject.Destroy (chargebar.gameObject);
	}

	// update charge bars
	void Update() {
		if (!BattleManager.main.enabled || BattleManager.main.pause) return;
		for (int i = 0; i < 3; i++) {
			if (charge_bars [i] != null) {
				Enemy enemy = BattleManager.main.enemy_arr [i];
				if (!enemy.Is_dead) {
					charge_bars [i].setValue (enemy.getProgress ());
					charge_bars [i].setText (enemy.getCurrSpell ().ToString ());
				} else { // if enemy has died, remove bar
					charge_bars[i].gameObject.SetActive(false);
				}
			}
		}
	}
}
