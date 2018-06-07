using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

// plays effect animations
public class AnimationPlayer : MonoBehaviour {
	public static AnimationPlayer main = null; // global static ref
	public bool ready; // are all of the assets loaded?
	public Animator camera_animator; // animator for main camera
	public GameObject animation_holder_prefab; // object prefab that holds the animations

	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
		if (main == null) main = this;
		else GameObject.Destroy (gameObject); // avoid multiple copies
		ready = true;
	}
		
	// plays specified animation with specified speed
	public void playAnimation(string name, Vector2 pos, float speed) {
		GameObject display = Instantiate (animation_holder_prefab, transform);
		display.transform.position = pos;
		SpriteRenderer sprite_r = display.GetComponent<SpriteRenderer> ();
		sprite_r.sortingOrder = 15; // put animation on top
		Animator animator = display.GetComponent<Animator>();
		animator.speed = speed;
		animator.Play (name, 0, 0f);
	}

	// plays specified screen effect
	public void playScreenEffect(string name) {
		camera_animator.Play (name);
	}
}
