using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// display's enemies' stagger bars
public class EnemyStaggerBars : MonoBehaviour {
	public GameObject staggerbar_prefab; // prefab for stagger bar
	public float bar_width; // width of bar
	Vector3 offset; // offset to account for width of bar
	BarMeter[] stagger_bars; // stagger bars for enemies

	//Get-only
	public BarMeter[] Stagger_bars
	{
		get
		{
			return stagger_bars;
		}
	}

	void Awake() {
		offset = new Vector3 (-0.5f * bar_width - 95.5f, -82.95f, 0);
		stagger_bars = new BarMeter[3];
	}

	// init stagger bars
	public void initStaggerBars() {
		if (stagger_bars != null)
			foreach (BarMeter bar in stagger_bars)
				if (bar != null) GameObject.Destroy(bar.gameObject);
		stagger_bars = new BarMeter[3];
		this.enabled = true;
	}

	// creates a new stagger meter for an enemy at world_pos
	// pos is the enemy's position in the enemt array (0, 1, 2)
	public void makeStaggerMeter(int pos, Vector3 world_pos) {
		GameObject new_bar = GameObject.Instantiate (staggerbar_prefab, transform);
		new_bar.transform.localScale = new Vector3 (1, 1, 1);
		new_bar.transform.position = world_pos;
		new_bar.transform.localPosition += offset;
		stagger_bars [pos] = new_bar.GetComponent<BarMeter> ();
		stagger_bars [pos].setText ("");
	}

	// removes all stagger bars
	public void removeAll() {
		this.enabled = false;
		foreach (BarMeter staggerbar in stagger_bars)
			if (staggerbar != null) GameObject.Destroy (staggerbar.gameObject);
	}

	// update stagger bars
	void Update() {
		if (!BattleManager.main.enabled || BattleManager.main.pause) return;
		for (int i = 0; i < 3; i++) {
			if (stagger_bars [i] != null) {
				Enemy enemy = BattleManager.main.enemy_arr [i];
				if (!enemy.Is_dead) {
					stagger_bars [i].setValue (enemy.getStagger ());
					if (enemy.Is_stunned) {
						stagger_bars[i].gameObject.SetActive(true);
					} else {
						stagger_bars[i].gameObject.SetActive(false);
					}
				} else { // if enemy has died, remove bar
					stagger_bars[i].gameObject.SetActive(false);
				}
			}
		}
	}
}
