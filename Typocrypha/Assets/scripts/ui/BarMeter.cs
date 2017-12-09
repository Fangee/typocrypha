using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// bar meter for representing health/charge/shield/etc
public class BarMeter : MonoBehaviour {
	float percent; // percent full
	float width; // width of max length of bar
	float height; // height of bar
	RectTransform bar; // bar's rect transform

	void Start () {
		percent = 1;
		width = GetComponent<RectTransform> ().rect.width;
		height = GetComponent<RectTransform> ().rect.height;
		bar = GetComponentInChildren<RectTransform> ();
	}

	// set bar length
	public void setValue(float new_percent) {
		percent = new_percent >= 0 ? new_percent : 0; // don't go past 0
		bar.localScale = new Vector3 (percent, 1, 1);
	}

}
