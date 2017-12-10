using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// causes various effects for battle scenes
public class BattleEffects : MonoBehaviour {
	public static BattleEffects main = null; // static global ref
	public Transform cam_pos; // main camera
	public SpriteRenderer dimmer; // dimmer image
	public Canvas canvas; // canvas component
	bool dimmed; // is scene currently dimmed?

	void Awake() {
		if (main == null) main = this;
		dimmed = false;
	}

	// dim screen (everything except target)
	// if target is null, dim everything except canvas
	public void toggleDim(SpriteRenderer target) {
		if (!dimmed) {
			if (target != null) target.sortingOrder = 10;
			else                canvas.sortingOrder = 10;
			dimmer.color = new Color (0, 0, 0, 0.5f);
			dimmed = true;
		} else {
			if (target != null) target.sortingOrder = 0;
			else                canvas.sortingOrder = 0;
			dimmer.color = new Color (0, 0, 0, 0);
			dimmed = false;
		}
	}

	// shake the screen for sec seconds
	public void screenShake(float sec) {
		StartCoroutine (screenShakeCR(sec));
	}

	// coroutine that shakes screen over time
	IEnumerator screenShakeCR(float sec) {
		float curr_time = 0;
		while (curr_time < sec) {
			cam_pos.position = Random.insideUnitCircle * 0.1f;
			yield return new WaitForSeconds(0.1f);
			curr_time += 0.1f;
		}
		cam_pos.position = new Vector3 (0, 0, 0);
	}
}
