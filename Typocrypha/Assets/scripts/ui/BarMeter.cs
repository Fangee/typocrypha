using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// bar meter for representing health/charge/shield/etc
public class BarMeter : MonoBehaviour {
	float percent; // percent full
	RectTransform bar; // bar's rect transform
	Text bar_text; // bar's text label

	void Awake () {
		bar = transform.GetChild(0).gameObject.GetComponent<RectTransform> ();
		bar_text = GetComponentInChildren<Text> ();
	}

	// set bar text
	public void setText(string new_txt) {
		bar_text.text = new_txt;
	}

	// set bar length
	public void setValue(float new_percent) {
		percent = new_percent >= 0 ? new_percent : 0; // don't go past 0
		bar.localScale = new Vector3 (percent, 1, 1);
	}

    // set bar color
    public void setColor(float r, float g, float b) {
        gameObject.transform.GetChild(0).GetComponent<Image>().color = new Color (r, g, b);
    }

}
