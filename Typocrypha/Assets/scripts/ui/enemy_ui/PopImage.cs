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
	public void display (string picName, float delay) {
		StartCoroutine (ShowPicture (picName, delay));
	}

	//ShowMessage coroutine
	IEnumerator ShowPicture (string picName, float delay) {
		img.sprite = Resources.Load<Sprite> (picName);
		img.rectTransform.sizeDelta = img.sprite.rect.size;
		yield return new WaitForSeconds (delay);
		Destroy (gameObject);
	}
}
