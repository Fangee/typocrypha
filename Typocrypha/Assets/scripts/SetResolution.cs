using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// sets resolution of screen
public class SetResolution : MonoBehaviour {
	public int width;
	public int height;

	void Start () {
		Screen.SetResolution (width, height, false);
	}
}
