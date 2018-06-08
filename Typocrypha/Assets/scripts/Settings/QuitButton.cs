using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour {

	public GameObject cam;
	SetResolution sr;
	Button quitbutt;

	// Use this for initialization
	void Start () {
		sr = cam.GetComponent<SetResolution>();
		quitbutt = GetComponent<Button>();

		quitbutt.onClick.AddListener(delegate{ ButtonPressed(); });
	}

	void ButtonPressed (){
		//sr.ApplySettings ();
		Application.Quit();
	}
}
