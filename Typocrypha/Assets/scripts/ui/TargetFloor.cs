using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// manages target floor panel effects
public class TargetFloor : MonoBehaviour {
	public GameObject[] floor_panels; // floor panels
	public SpriteRenderer[] highlights; // highlights of panels

	// update floor panels based on targetted enemy
	public void updateFloor() {
		for (int i = 0; i < BattleManagerS.main.field.enemy_arr.Length; ++i) {
			// highlight selected enemy
			highlights [i].enabled = (i == BattleManagerS.main.field.target_ind);
			// show/hide floors depending on target
			floor_panels[i].SetActive(true);
			//floor_panels[i].SetActive(BattleManagerS.main.field.enemy_arr[i] != null && !BattleManagerS.main.field.enemy_arr[i].Is_dead);
		}
	}
}
