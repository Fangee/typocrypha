using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//script for the PopText prefab
public class PopText : MonoBehaviour {

	public FXText txt;
	TextGradientFill text_fill;
	RectTransform text_rect;
	[HideInInspector] public List<FXTextEffect> fx_text_effects; // List of text effects
	[HideInInspector] public List<TextEvent>[] text_events; // Array of text events at each character index

	void Awake () {
		txt = GetComponent<FXText> ();
		text_fill = GetComponent<TextGradientFill> ();
		text_rect = GetComponent<RectTransform> ();
        txt.alignment = TextAnchor.MiddleCenter;
	}

	//print message lasting for the specified time
	public void display (string message, float delay) {
		StartCoroutine (Bleh (message, delay));
	}

	//change text color (singular)
	public void setColor(Color color){
		txt.color = color;
		if (text_fill != null) {
			text_fill.topColor = color;
			text_fill.bottomColor = color;
		}
	}

	//change text color (gradient)
	public void setColor(Color colorTop, Color colorBottom){
		txt.color = colorTop;
		if (text_fill != null) {
			text_fill.topColor = colorTop;
			text_fill.bottomColor = colorBottom;
		}
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
        /*for (int i = 0; i < 6; i++)
        {
			txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, (i + 1) / 6);
			Vector3 scaler = new Vector3 ((float)(i / 5f),text_rect.localScale.y,text_rect.localScale.z);
			text_rect.localScale = scaler;
			yield return new WaitForEndOfFrame();
        }*/
		StartCoroutine (WipeIn());
        yield return new WaitForSeconds (delay);
		/*for (int i = 0; i < 6; i++)
		{
			Vector3 scaler = new Vector3 ((float)(i + 1),(text_rect.localScale.y)/(i + 1),text_rect.localScale.z);
			text_rect.localScale = scaler;
			yield return new WaitForEndOfFrame();
		}*/
		StartCoroutine (WipeOut());
        //Destroy (gameObject);
    }

	IEnumerator WipeIn () {
		for(int i = 1; i < 7; i++)
		{
			txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, (i + 1) / 7);
			Vector3 scaler = new Vector3 ((float)(i / 6f),text_rect.localScale.y,text_rect.localScale.z);
			text_rect.localScale = scaler;
			yield return new WaitForEndOfFrame();
		}
	}

	IEnumerator WipeOut () {
		for(int i = 6; i > 0; i--)
		{
			Vector3 scaler = new Vector3 ((float)text_rect.localScale.x*(16-i)/10,((text_rect.localScale.y)*i)/10,text_rect.localScale.z);
			text_rect.localScale = scaler;
			yield return new WaitForEndOfFrame();
		}
		Destroy (gameObject);
	}
}
