using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Apply alpha changes to all SpriteRenderers in object hiearchy
// Grabs all SpriteRenderer references on Start; doesn't auto update
[ExecuteInEditMode]
public class AlphaOfHiearchy : MonoBehaviour {
	public float alpha;

	float prev_alpha; // alpha in previous frame
	SpriteRenderer[] spr_array;

	void Start() {
		updateSpriteRenderers ();
		prev_alpha = alpha;
	}

	void LateUpdate() {
		if (alpha != prev_alpha) {
			foreach (SpriteRenderer spr in spr_array)
				spr.color = new Color (spr.color.r, spr.color.g, spr.color.b, alpha);
			prev_alpha = alpha;
		}
	}

	public void updateSpriteRenderers() {
		spr_array = GetComponentsInChildren<SpriteRenderer> ();
	}
}
