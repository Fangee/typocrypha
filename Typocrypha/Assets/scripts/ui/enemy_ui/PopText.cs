using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//script for the PopText prefab
public class PopText : MonoBehaviour {

	Text txt;

	void Awake () {
		txt = GetComponent<Text> ();
        txt.alignment = TextAnchor.MiddleCenter;
	}

	//print message lasting for the specified time
	public void display (string message, float delay) {
		StartCoroutine (Bleh (message, delay));
	}

	//change text color
	public void setColor(Color color){
		txt.color = color;
	}

	//ShowMessage coroutine
	IEnumerator ShowMessage (string message, float delay) {
		txt.text = message;
		yield return new WaitForSeconds (delay);
		Destroy (gameObject);
	}

    IEnumerator Bleh (string message, float delay) {
        txt.text = message;
        Vector3 beef = transform.position;
        for (int i = 0; i < 6; i++)
        {
            transform.position = beef + new Vector3 (Random.value/3, Random.value/3, 0);
            yield return new WaitForEndOfFrame();
        }
        transform.position = beef;
        yield return new WaitForSeconds (delay);
        Destroy (gameObject);
    }

}
