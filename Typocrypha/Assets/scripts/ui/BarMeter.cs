using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// bar meter for representing health/charge/shield/etc
public class BarMeter : MonoBehaviour {
	float percent; // percent full
	RectTransform bar; // bar's rect transform

	void Start () {
		bar = transform.GetChild(0).gameObject.GetComponent<RectTransform> ();
	}

	// set bar length
	public void setValue(float new_percent) {
		percent = new_percent >= 0 ? new_percent : 0; // don't go past 0
		bar.localScale = new Vector3 (percent, 1, 1);
	}

}
