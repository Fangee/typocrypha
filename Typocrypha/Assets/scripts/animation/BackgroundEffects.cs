using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// performs various background effects
public class BackgroundEffects : MonoBehaviour {
	public static BackgroundEffects main;
	public WavySprite wavy_sprite;
	public SpriteRenderer bg_dimmer;

	GameObject prefab_bg; // background object if loaded from prefab

	const int bg_layer = -10;

	void Start() {
		if (main == null) main = this;
		wavy_sprite.orderInLayer = bg_layer;
	}

	// sets sprite of background image
	public void setSpriteBG(string sprite_name) {
		wavy_sprite.texture = Resources.Load<Texture2D> ("sprites/backgrounds/" + sprite_name);
	}

	// sets prefab background object as background
	public void setPrefabBG(string prefab_name) {
		prefab_bg = Instantiate (Resources.Load<GameObject> ("prefabs/" + prefab_name), this.transform);
	}

	// sets prefab background object as background with direct reference
	public void setPrefabBG(GameObject prefab) {
		prefab_bg = Instantiate (prefab, this.transform);
	}

	// removes current prefab background after delay
	public void removePrefabBG(float delay) {
		Destroy (prefab_bg, delay);
		prefab_bg = null;
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
		for (;;) {
			if (frames.Count == 0 || frames[frames.Count - 1].position.x > 0f) {
				GameObject n_frame = new GameObject ();
				SRWavySprite n_frame_r = n_frame.AddComponent<SRWavySprite> ();
				n_frame_r.texture = wavy_sprite.texture;
				n_frame_r.sortingLayer = bg_layer;
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
