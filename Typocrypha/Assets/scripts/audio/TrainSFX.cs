using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// manages train audio and sfx
public class TrainSFX : MonoBehaviour {
	public Transform train_tr; // transform of train
	public AudioSource ambient; // ambient audio source
	public AudioSource bump; // train bump sound effect 

	void Start() {
		StartCoroutine (checkForBumps ());
	}

	IEnumerator checkForBumps() {
		while (true) {
			if (train_tr.localPosition.y > 3) {
				bump.Play ();
				yield return new WaitForSeconds (0.4f);
			}
			yield return null;
		}
	}
}
