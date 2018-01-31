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

	// coroutine that starts left/right movement (infinitely)
	// if speed is negative, moves left, otherwise, right
	IEnumerator conveyorBG_cr(float speed) {
		List<Transform> frames = new List<Transform> ();
		GameObject frame = new GameObject ();
		SpriteRenderer frame_r = frame.AddComponent<SpriteRenderer> ();
		frame_r.sortingOrder = -5;
		frame_r.sprite = sprite_r.sprite;
		frames.Add (frame.transform);
		for (;;) {
			if (frames.Count == 0 || frames[frames.Count - 1].position.x > 0f) {
				GameObject n_frame = new GameObject ();
				SpriteRenderer n_frame_r = n_frame.AddComponent<SpriteRenderer> ();
				n_frame_r.sprite = sprite_r.sprite;
				n_frame_r.sortingOrder = -10;
				n_frame.transform.position = new Vector2 (-20f, 0);
				frames.Add (n_frame.transform);
			}
			foreach (Transform tr in frames)
				tr.Translate (speed, 0, 0);
			if (frames [0].position.x > 20f) {
				GameObject old_frame = frames [0].gameObject;
				frames.RemoveAt (0);
				Destroy (old_frame);
			}
			yield return new WaitForEndOfFrame ();
		}
	}
}
