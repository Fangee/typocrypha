using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseCanvas : MonoBehaviour {
	public static PauseCanvas main = null;

	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
		if (main == null) main = this;
		else GameObject.Destroy (gameObject); // avoid multiple copies
	}
}
