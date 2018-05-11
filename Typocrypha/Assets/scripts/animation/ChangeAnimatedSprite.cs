using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Allows attached object to change sprite while animated
public class ChangeAnimatedSprite : MonoBehaviour {
	public SpriteRenderer spr_rend;
	Sprite sprite;

	void LateUpdate () {
		if (sprite != null) spr_rend.sprite = sprite;
	}

	public void changeSprite(Sprite _sprite) {
		sprite = _sprite;
	}
}
