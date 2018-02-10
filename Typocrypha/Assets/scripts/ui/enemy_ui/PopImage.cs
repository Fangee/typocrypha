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
	public void display (Sprite spr, float dura) {
        img.sprite = spr;
        img.rectTransform.sizeDelta = Vector2.zero;
        StartCoroutine (ShowPicture (dura));
	}

	//ShowMessage coroutine
    IEnumerator ShowPicture (float dura) {
        StartCoroutine (WipeIn());
		yield return new WaitForSeconds (dura);
        StartCoroutine (WipeOut());
	}

    IEnumerator WipeIn () {
        for(int i = 1; i < 7; i++)
        {
            img.rectTransform.sizeDelta = new Vector2 (img.sprite.rect.size.x*i/10, img.sprite.rect.size.y);
            yield return new WaitForEndOfFrame();
        }
        img.rectTransform.sizeDelta = img.sprite.rect.size;
    }

    IEnumerator WipeOut () {
        for(int i = 6; i > 0; i--)
        {
            img.rectTransform.sizeDelta = new Vector2 (img.sprite.rect.size.x*(16-i)/10, img.sprite.rect.size.y*i/10);
            yield return new WaitForEndOfFrame();
        }
        Destroy (gameObject);
    }
}
