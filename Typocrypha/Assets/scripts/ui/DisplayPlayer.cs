using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// displays player's stats in appropiate areas
public class DisplayPlayer : MonoBehaviour {
	public Text health_text; // display's player's health
	public Text shield_text; // display's player's shield
	public Text cast_text; // display's what player is casting

	void Update () {
		health_text.text = Player.curr_hp.ToString ();
		shield_text.text = Player.shield.ToString ();
		cast_text.text = Player.last_cast;
	}
}
