using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// performs various background effects
public class BackgroundEffects : MonoBehaviour {
	public static BackgroundEffects main;
	public SpriteRenderer sprite_r;
	public SpriteRenderer bg_dimmer;

	void Start() {
		if (main == null) main = this;
	}

	// sets sprite of background image
	public void setBG(string sprite_name) {
		sprite_r.sprite = Resources.Load<Sprite> ("sprites/backgrounds/" + sprite_name);
	}

	// fades current background image in/out over 'fade_time' seconds into color 'fade_color'
	public void fadeBG(bool fade_in, float fade_time, Color fade_color) {
		StartCoroutine (fadeBG_cr (fade_in, fade_time, fade_color));
	}

	// coroutine that fades current background image
	IEnumerator fadeBG_cr(bool fade_in, float fade_time, Color fade_color) {
		// TODO
		yield return true;
	}
}
