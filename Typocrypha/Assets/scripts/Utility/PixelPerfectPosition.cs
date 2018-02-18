using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// adjust position of attached slightly for pixel perfection (aligned to pixels)
public class PixelPerfectPosition : MonoBehaviour {
	public bool update = false; // should position constantly be readjusted?

	Camera main_camera; // main camera
	float bound; // the size of one pixel in world coordinates

	void Start () {
		// get height and width of world (in world units)
		main_camera = GameObject.Find("Main Camera").GetComponent<Camera>();
		float h = main_camera.orthographicSize * 2f;
		bound = h / Screen.height;
		float x = transform.position.x - (transform.position.x % bound);
		float y = transform.position.y - (transform.position.y % bound);
		transform.position = new Vector2 (x, y);
	}

	void LateUpdate() {
		if (update) {
			float x = transform.position.x - (transform.position.x % bound);
			float y = transform.position.y - (transform.position.y % bound);
			transform.position = new Vector2 (x, y);
		}
	}
}
