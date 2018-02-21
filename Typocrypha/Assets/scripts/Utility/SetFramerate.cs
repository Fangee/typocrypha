using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// sets framerate properties
public class SetFramerate : MonoBehaviour {
	public int frameRate;

	void Awake() {
		Application.targetFrameRate = frameRate;
	}
}
