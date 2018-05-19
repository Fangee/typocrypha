using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// sets resolution of screen
public class SetResolution : MonoBehaviour {
	public int width;
	public int height;
	public bool full;

	void Start () {
		ApplySettings ();
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
