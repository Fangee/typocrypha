using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// adjust position of attached slightly for pixel perfection (aligned to pixels)
public class PixelPerfectPosition : MonoBehaviour {
	void Start () {
		// get height and width of world (in world units)
		Camera main_camera = GameObject.Find("Main Camera").GetComponent<Camera>();
		float h = main_camera.orthographicSize * 2f;
		float bound = h / Screen.height;
		float x = transform.position.x - (transform.position.x % bound);
		float y = transform.position.y - (transform.position.y % bound);
		transform.position = new Vector2 (x, y);
		Debug.Log (x + " " + y);
	}
}
