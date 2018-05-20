using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResFullscreenToggle : MonoBehaviour {

	public GameObject cam;
	SetResolution sr;
	Toggle fulltog;

	// Use this for initialization
	void Start () {
		sr = cam.GetComponent<SetResolution>();
		fulltog = GetComponent<Toggle>();
		fulltog.isOn = sr.full;

		fulltog.onValueChanged.AddListener(delegate{ TogglePressed(fulltog); });
	}

	void TogglePressed (Toggle t){
		sr.SetFull (t.isOn);
	}
}
