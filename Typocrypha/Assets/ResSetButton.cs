using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResSetButton : MonoBehaviour {

	public GameObject cam;
	SetResolution sr;
	Button resbutt;

	// Use this for initialization
	void Start () {
		sr = cam.GetComponent<SetResolution>();
		resbutt = GetComponent<Button>();

		resbutt.onClick.AddListener(delegate{ ButtonPressed(); });
	}
	
	void ButtonPressed (){
		sr.ApplySettings ();
	}
}
