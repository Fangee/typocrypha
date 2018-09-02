using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// floating text management
public class FloatText : MonoBehaviour {
	public void startFloatText(float x, float y, string text) {
		StartCoroutine (startFloatTextCR (x, y, text));
	}

	IEnumerator startFloatTextCR(float x, float y, string text) {
        DialogItem d_item = new DialogItem(text);//gameObject.AddComponent<DialogueItemAN>();
		transform.position = new Vector2 (x, y);
		DialogBox d_box = GetComponent<DialogBox> ();
		d_box.is_floating = true;
		d_box.talk_sfx = false;
		d_item.text = text;
		d_box.d_item = d_item;
        d_box.scrollDelay = 0.07f;
		// parse effects and macros
		d_box.text = DialogParser.main.parse (d_item, d_box);
		foreach(FXTextEffect text_effect in d_item.fx_text_effects)
			d_box.fx_text.addEffect (text_effect);
		// start text scroll
		d_box.dialogBoxStart (null);
		// wait for dialogue to finish scrolling
		yield return new WaitUntil (() => d_box.cr_scroll != null);
		yield return new WaitWhile (() => d_box.cr_scroll != null);
		// wait for a little more and then fade out
		yield return new WaitForSeconds(1f);
		FXTextColor fade = d_box.fx_text.gameObject.AddComponent<FXTextColor>();
		fade.color = d_box.fx_text.color;
		fade.chars = new int[2]{0,d_box.text.Length};
		d_box.fx_text.addEffect (fade);
		while (d_box.fx_text.color.a > 0) {
			fade.color.a -= 0.03f;
			yield return null;
		}
		Destroy (gameObject);
	}
}
