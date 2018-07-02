using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// sets resolution of screen
public class SetResolution : MonoBehaviour {
	public static SetResolution main = null;
	public int width;
	public int height;
	public bool full;

	void Awake () {
		DontDestroyOnLoad(transform.gameObject);
		if (main == null) {
			main = this;
			ApplySettings ();
		} else {
			GameObject.Destroy (gameObject); // avoid multiple copies
		}
	}

	public void SetRes (int w, int h) {
		width = w;
		height = h;
    }

	public void SetFull (bool f) {
		full = f;
	}

	public void ApplySettings () {
		Screen.SetResolution (width, height, full);
	}

}
