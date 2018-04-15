using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// manages target floor panel effects
public class TargetFloor : MonoBehaviour {
	public GameObject[] floor_panels; // floor panels
	public SpriteRenderer[] highlights; // highlights of panels

	// update floor panels based on targetted enemy
	public void updateFloor() {
		for (int i = 0; i < BattleManager.main.battle_field.enemy_arr.Length; ++i) {
			// highlight selected enemy
			highlights [i].enabled = (i == BattleManager.main.battle_field.target_ind);
			// show/hide floors depending on target
			floor_panels[i].SetActive(BattleManager.main.battle_field.enemy_arr[i] != null && !BattleManager.main.battle_field.enemy_arr[i].Is_dead);
		}
	}
}
