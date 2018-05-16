using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//script for the PopText prefab
public class PopText : MonoBehaviour {

	public FXText txt;
	TextGradientFill text_fill;
	[HideInInspector] public List<FXTextEffect> fx_text_effects; // List of text effects
	[HideInInspector] public List<TextEvent>[] text_events; // Array of text events at each character index

	void Awake () {
		txt = GetComponent<FXText> ();
		text_fill = GetComponent<TextGradientFill> ();
        txt.alignment = TextAnchor.MiddleCenter;
	}

	//print message lasting for the specified time
	public void display (string message, float delay) {
		StartCoroutine (Bleh (message, delay));
	}

	//change text color (singular)
	public void setColor(Color color){
		txt.color = color;
		text_fill.topColor = color;
		text_fill.bottomColor = color;
	}

	//change text color (gradient)
	public void setColor(Color colorTop, Color colorBottom){
		txt.color = colorTop;
		text_fill.topColor = colorTop;
		text_fill.bottomColor = colorBottom;
	}

	//ShowMessage coroutine
	IEnumerator ShowMessage (string message, float delay) {
		txt.text = message;
		yield return new WaitForSeconds (delay);
		Destroy (gameObject);
	}

    IEnumerator Bleh (string message, float delay) {
        txt.text = message;
        BattleEffects.main.spriteShake(transform, 0.1f, 0.2f);
        for (int i = 0; i < 6; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds (delay);
        Destroy (gameObject);
    }

}
