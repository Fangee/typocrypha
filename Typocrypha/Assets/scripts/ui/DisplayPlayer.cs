using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// displays player's stats in appropiate areas
public class DisplayPlayer : MonoBehaviour {
	public Text health_text; // display's player's health
	public Text name_text; // displays player's name
	public BarMeter health_bar; // health bar

	void Update () {
		health_text.text = (Player.main.Curr_hp).ToString ();
		name_text.text = PlayerDataManager.main.PlayerName.ToString ().ToUpper ();
		health_bar.setValue ((float)(Player.main.Curr_hp)/(Player.main.Stats.max_hp+Player.main.Stats.max_shield));
	}
}
