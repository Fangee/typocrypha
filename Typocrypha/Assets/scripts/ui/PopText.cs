using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//script for the PopText prefab
public class PopText : MonoBehaviour {

	Text txt;

	void Awake () {
		txt = GetComponent<Text> ();
	}

	//print message lasting for the specified time
	public void display (string message, float delay) {
		StartCoroutine (ShowMessage (message, delay));
	}

	//ShowMessage coroutine
	IEnumerator ShowMessage (string message, float delay) {
		txt.text = message;
		yield return new WaitForSeconds (delay);
		Destroy (gameObject);
	}

}
