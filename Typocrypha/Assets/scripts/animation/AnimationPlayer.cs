using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

// plays effect animations
public class AnimationPlayer : MonoBehaviour {
	public static AnimationPlayer main = null; // global static ref
	public bool ready; // are all of the assets loaded?
	public Animator camera_animator; // animator for main camera
	public Animator flash_animator; // animator for white flashes
	public GameObject animation_holder_prefab; // object prefab that holds the animations

    public Transform status_effects; // parent object to all status effect animations
    public GameObject volt_prefab; // prefab for volt status effect animation object

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

	// plays specified flash effect
	public void playFlashEffect(string name) {
		flash_animator.Play (name);
	}

    // displays volt status effect animation at specific location (runs for 10 seconds)
    public void playStatusEffectVolt(Vector2 pos, string c)
    {
        GameObject display = Instantiate(volt_prefab, status_effects);
        display.GetComponentInChildren<Text>().text = c;
        display.transform.position = pos;
        display.SetActive(true);
        display.transform.Translate(new Vector3(0.3765f, -0.3765f, 0f));
    }
}
