using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//script for object that handles pop-up graphics
public class Popper : MonoBehaviour {

	public GameObject poptext_prefab;
	GameObject txtObj;
	PopText pt;

	public GameObject popsprite_prefab;
	GameObject sprObj;
	PopSprite ps;


	public void spawnText (string message, float delay, Vector3 pos) {
		txtObj = GameObject.Instantiate (poptext_prefab, transform);
		txtObj.transform.position = pos;
		pt = txtObj.GetComponent<PopText>();
		pt.display (message, delay);
	}

	public void spawnSprite (string picName, float delay, Vector3 pos) {
		sprObj = GameObject.Instantiate (popsprite_prefab, transform);
		sprObj.transform.position = pos;
		ps = sprObj.GetComponent<PopSprite>();
		ps.display (picName, delay);
	}

}
