using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// bar meter for representing health/charge/shield/etc
public class BarMeter : MonoBehaviour {
	float percent; // percent full
	RectTransform bar; // bar's rect transform
	RectTransform bar_afterimage; // bar's afterimage rect transform
	Text bar_text; // bar's text label

	void Awake () {
		bar = transform.GetChild(1).gameObject.GetComponent<RectTransform> ();
		bar_afterimage = transform.GetChild(0).gameObject.GetComponent<RectTransform> ();
		bar_text = GetComponentInChildren<Text> ();
	}

	// set bar text
	public void setText(string new_txt) {
		bar_text.text = new_txt;
	}

	// set bat text's color
	public void setTextColor(Color new_color){
		bar_text.color = new_color;
	}

	// set bar length
	public void setValue(float new_percent) {
		percent = new_percent >= 0 ? new_percent : 0; // don't go past 0
		bar.localScale = new Vector3 (percent, 1, 1);
	}

	// set bar afterimage length
	public void setAfterimage(float new_percent) {
		percent = new_percent >= 0 ? new_percent : 0; // don't go past 0
		bar_afterimage.localScale = new Vector3 (percent, 1, 1);
	}

	// get the local scale of the bar as a Vector3
	public Vector3 getBarLocalScale(){
		return bar.localScale;
	}

    // set bar color
    public void setColor(float r, float g, float b)
    {
        gameObject.transform.GetChild(1).GetComponent<Image>().color = new Color(r, g, b);
    }

}
