using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// display's enemies' health bars
public class EnemyHealthBars : MonoBehaviour {
	public GameObject healthbar_prefab; // prefab for health bar
	public float bar_width; // width of bar
	Vector3 offset; // offset to account for width of bar
	BarMeter[] health_bars; // health bars for enemies

	//Get-only
	public BarMeter[] Health_bars
	{
		get
		{
			return health_bars;
		}
	}

	void Awake() {
		offset = new Vector3 (-0.5f * bar_width - 95.5f, -82.95f, 0);
		health_bars = new BarMeter[3];
	}

	// init health bars
	public void initHealthBars() {
		if (health_bars != null)
			foreach (BarMeter bar in health_bars)
				if (bar != null) GameObject.Destroy(bar.gameObject);
		health_bars = new BarMeter[3];
		this.enabled = true;
	}

	// creates a new health meter for an enemy at world_pos
	// pos is the enemy's position in the enemt array (0, 1, 2)
	public void makeHealthMeter(int pos, Vector3 world_pos) {
		GameObject new_bar = GameObject.Instantiate (healthbar_prefab, transform);
		new_bar.transform.localScale = new Vector3 (1, 1, 1);
		new_bar.transform.position = world_pos;
		new_bar.transform.localPosition += offset;
		health_bars [pos] = new_bar.GetComponent<BarMeter> ();
		health_bars [pos].setText ("");
	}

	// removes all health bars
	public void removeAll() {
		this.enabled = false;
		foreach (BarMeter healthbar in health_bars)
			if (healthbar != null) GameObject.Destroy (healthbar.gameObject);
	}

	// update health bars
	void Update() {
		if (!BattleManagerS.main.enabled) return;
		for (int i = 0; i < 3; i++) {
			if (health_bars [i] != null) {
				Enemy enemy = BattleManagerS.main.field.enemies [i];
				if (!enemy.Is_dead) {
                    if (!BattleManagerS.main.battlePause) {
                        health_bars[i].setValue(((float)enemy.Curr_hp / (float)enemy.Stats.max_hp));
                        health_bars[i].setAfterimage(((float)enemy.Curr_hp / (float)enemy.Stats.max_hp));
                    }
				} else { // if enemy has died, remove bar
					health_bars[i].setValue(0);
					health_bars[i].setAfterimage(0);
					//health_bars[i].gameObject.SetActive(false);
				}
			}
		}
	}

	// function that gradually lowers the HP bar
	public IEnumerator gradualUpdateDamage(int enemyIndex, float damage){
		Enemy enemy = BattleManagerS.main.field.enemies [enemyIndex];
        if (enemy == null || enemy.Is_dead)
            yield break;
		float old_x = health_bars [enemyIndex].getBarLocalScale().x;
		health_bars[enemyIndex].setAfterimage(old_x);
		health_bars[enemyIndex].setValue(((float)enemy.Curr_hp / (float)enemy.Stats.max_hp));
		float hpCurrRatio = (float)enemy.Curr_hp / (float)enemy.Stats.max_hp; // the ratio of maxhp that currhp is
		float hpDiffRatio = Mathf.Min(damage / (float)enemy.Stats.max_hp, 1.0f); // the ratio of maxhp that damage is
		float temp = hpDiffRatio / 10;
		for (float i = hpDiffRatio; i+hpCurrRatio >= hpCurrRatio;)
		{
            if (enemy == null || enemy.Is_dead)
                yield break;
            yield return new WaitForEndOfFrame();
			health_bars[enemyIndex].setAfterimage( Mathf.Min(hpCurrRatio + hpDiffRatio, old_x));
			hpDiffRatio = hpDiffRatio - temp;
			//if (hpDiff > 0 && itor <= hpDiff) {
				//health_bars[enemyIndex].setAfterimage(Mathf.Max(((float)enemy.Stats.max_hp - itor / (float)enemy.Stats.max_hp), 0));
				//++itor;
			//}
		}
		yield return true;
	}
}
