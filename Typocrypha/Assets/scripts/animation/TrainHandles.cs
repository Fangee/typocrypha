using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// controls all train handles together
public class TrainHandles : MonoBehaviour {
	public float alpha; // global alpha for all handles

	void Update () {
		foreach (Transform child in transform) {
			SpriteRenderer spr = child.gameObject.GetComponentInChildren<SpriteRenderer> ();
			spr.color = new Color (spr.color.r, spr.color.g, spr.color.b, alpha);
		}
	}
}
