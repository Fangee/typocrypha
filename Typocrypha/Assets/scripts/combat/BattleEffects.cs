using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// edits by James Iwamasa

// causes various effects for battle scenes
public class BattleEffects : MonoBehaviour {
	public static BattleEffects main = null; // static global ref
	public Transform cam_pos; // main camera
	public SpriteRenderer dimmer; // dimmer image
	public Canvas canvas; // canvas component

	void Awake() {
		if (main == null) main = this;
	}

	// turn dim on/off
	public void setDim(bool dim, SpriteRenderer target) {
		if (dim) {
			if (target != null) target.sortingOrder = 10;
			dimmer.color = new Color (0, 0, 0, 0.5f);
		} else {
			if (target != null) target.sortingOrder = 0;
			dimmer.color = new Color (0, 0, 0, 0);
		}
	}

	// shake the screen for sec seconds and amt intensity
	public void screenShake(float sec, float amt) {
		StartCoroutine (screenShakeCR(sec, amt));
	}

	// coroutine that shakes screen over time
	IEnumerator screenShakeCR(float sec, float amt) {
		Vector3 old_pos = cam_pos.position;
		float curr_time = 0;
		while (curr_time < sec) {
			cam_pos.position = Random.insideUnitCircle * amt;
			yield return new WaitForEndOfFrame();
			curr_time += Time.deltaTime;
		}
		cam_pos.position = old_pos;
	}

	// shake a sprite for sec seconds and amt intensity
	public void spriteShake(Transform pos, float sec, float amt) {
		StartCoroutine(spriteShakeCR (pos, sec, amt));
	}

	// coroutine that shakes sprite over time
	IEnumerator spriteShakeCR(Transform pos, float sec, float amt) {
		Vector3 old_pos = pos.position;
		float curr_time = 0;
		while (curr_time < sec) {
			pos.position = (Vector3)old_pos + (Vector3)(Random.insideUnitCircle * amt);
			for (int i = 0; i < 4; ++i) {
				yield return new WaitForEndOfFrame ();
				curr_time += Time.deltaTime;
			}
		}
		pos.position = old_pos;
	}
}
