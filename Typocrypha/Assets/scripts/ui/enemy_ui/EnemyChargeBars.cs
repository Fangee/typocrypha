using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// displays enemies' charge bars for attacks
public class EnemyChargeBars : MonoBehaviour {
	public GameObject chargebar_prefab; // prefab for charge bar
	public float bar_width; // width of bar
	Vector3 offset; // offset to account for width of bar
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
		offset = new Vector3 (-0.5f * bar_width - 109f, -69.9f, 0); // place halfway (centered)
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
		new_bar.transform.localPosition += offset;
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
		if (!BattleManagerS.main.enabled || BattleManagerS.main.pause) return;
		updateChargeBars ();
	}

	public void updateChargeBars() {
		for (int i = 0; i < 3; i++) {
			if (charge_bars [i] != null) {
				Enemy enemy = BattleManagerS.main.field.enemy_arr [i];
				Text stagger_value = charge_bars [i].transform.GetChild (5).GetComponent<Text> ();
				Color color_stagger_full = new Color (13.0f / 255.0f, 207.0f / 255.0f, 223.0f / 255.0f);
				Color color_not_stunned = new Color (255.0f, 110.0f / 255.0f, 255.0f);
				Color color_stunned = new Color (242.0f / 255.0f, 48.0f / 255.0f, 32.0f / 255.0f);
				charge_bars [i].transform.GetChild (6).GetComponent<Text> ().text = enemy.Stats.name;
				if (!enemy.Is_dead) {
					charge_bars [i].setValue (enemy.getAtkProgress ());
					stagger_value.text = (enemy.Curr_stagger.ToString());
					if (enemy.Is_stunned) {
						charge_bars[i].setText ("> STAGGERED!");
						charge_bars [i].setTextColor (color_stunned);
						charge_bars[i].transform.GetChild (3).GetComponent<Image> ().enabled = false;
						charge_bars[i].transform.GetChild (4).GetComponent<Image> ().enabled = true;
						stagger_value.color = color_stunned;
					} else {
						charge_bars [i].setText ("> " + enemy.getCurrSpell ().ToString ());
						charge_bars [i].setTextColor (color_not_stunned);
						charge_bars[i].transform.GetChild (3).GetComponent<Image> ().enabled = true;
						charge_bars[i].transform.GetChild (4).GetComponent<Image> ().enabled = false;
						if ((float)enemy.Curr_stagger / (float)enemy.Stats.max_stagger < 0.5f) {
							stagger_value.color = Color.yellow;
						}
						else if ((float)enemy.Curr_stagger / (float)enemy.Stats.max_stagger < 1.0f){
							stagger_value.color = Color.white;
						} 
						else {
							stagger_value.color = color_stagger_full;
						}
					}
				} else { // if enemy has died, remove bar
					charge_bars[i].gameObject.SetActive(false);
				}
			}
		}
	}
}
