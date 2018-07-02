using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScreenframeInfo : MonoBehaviour {

	public Text username; // player's name
	public Text currentTime; // current system time

	// Update is called once per frame
	void Update () {
		System.DayOfWeek dayOfWeekSystem = System.DateTime.Now.DayOfWeek;
		string dayOfWeek = dayOfWeekSystem.ToString ();
		string time = System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute;
		string date = System.DateTime.Now.Month + " / " + System.DateTime.Now.Day + " / " + System.DateTime.Now.Year;
		if (!Pause.main.title)
			username.text = "U: " + PlayerDialogueInfo.main.player_name;
		currentTime.text = dayOfWeek + " " + time + "\n" + date;
	}
}
