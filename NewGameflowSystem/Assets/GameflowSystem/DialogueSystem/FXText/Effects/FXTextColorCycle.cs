using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Text random color cycle effect
public class FXTextColorCycle : FXTextEffect {
	public float period = 1f; // how long between color changes
	public int color_count = 1; // how many different colors to cycle through

	Color[] colors; // list of colors

	public override void initEffect () {
		color_count = 16;
		period = 0.1f;
		colors = new Color[color_count];
		StartCoroutine (colorSwitch ());
	}

	public override void stepEffect (UIVertex[] stream) {
		for (int i = 0; i < chars.Length; i+=2) {
			for (int j = chars[i]; j < chars[i+1]; ++j) {
				int k = j * 6;
				for (int c = 0; c < 6; ++c)
					stream [k + c].color = colors[j % color_count];
			}
		}
	}

	public override void stopEffect (UIVertex[] stream) {
		StopAllCoroutines ();
	}

	// switches colors every period
	IEnumerator colorSwitch() {
		for (;;) {
			for (int i = 0; i < color_count; ++i)
				colors [i] = Random.ColorHSV ();
			yield return new WaitForSeconds (period);
		}
	}

	public override FXTextEffect clone (GameObject obj)
	{
		FXTextColorCycle f = obj.AddComponent<FXTextColorCycle>();
		f.color_count = color_count;
		f.period = period;
		return f;
	}
}

