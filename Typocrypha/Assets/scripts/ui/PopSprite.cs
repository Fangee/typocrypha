using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//script for the PopSprite prefab
public class PopSprite : MonoBehaviour {

	SpriteRenderer spr;

	void Awake () {
		spr = GetComponent<SpriteRenderer> ();
	}

	//print message lasting for the specified time
	public void display (string picName, float delay) {
		StartCoroutine (ShowPicture (picName, delay));
	}

	//ShowMessage coroutine
	IEnumerator ShowPicture (string picName, float delay) {
		spr.sprite = Resources.Load<Sprite> (picName);
		transform.localScale = new Vector3 (75,75,1);
		yield return new WaitForSeconds (delay);
		Destroy (gameObject);
	}
}
