using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//script for the PopSprite prefab
public class PopImage : MonoBehaviour {

	Image img;

	void Awake () {
		img = GetComponent<Image> ();
	}

	//display image lasting for the specified time
	public void display (Sprite spr, float delay) {
		StartCoroutine (ShowPicture (spr, delay));
	}

	//ShowMessage coroutine
    IEnumerator ShowPicture (Sprite spr, float delay) {
        img.sprite = spr;
		img.rectTransform.sizeDelta = img.sprite.rect.size;
		yield return new WaitForSeconds (delay);
		Destroy (gameObject);
	}
}
