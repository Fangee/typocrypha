using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

// plays effect animations
public class AnimationPlayer : MonoBehaviour {
	public static AnimationPlayer main = null; // global static ref
	public Animator camera_animator; // animator for main camera
	public Animator flash_animator; // animator for white flashes
	public GameObject animation_holder_prefab; // object prefab that holds the animations

    [HideInInspector] public bool ready; // are all of the assets loaded?

    void Awake() {
		DontDestroyOnLoad(transform.gameObject);
		if (main == null) main = this;
		else GameObject.Destroy (gameObject); // avoid multiple copies
		ready = true;
	}
	
    //Plays any one-shot animation clip and returns the play time as a float
    public float playAnimation(AnimationClip clip, Vector2 pos, float speed = 1f)
    {
        GameObject display = Instantiate(animation_holder_prefab, transform);
        display.transform.position = pos;
        SpriteRenderer sprite_r = display.GetComponent<SpriteRenderer>();
        sprite_r.sortingOrder = 15; // put animation on top
        Animator animator = display.GetComponent<Animator>();
        animator.speed = speed;
        AnimatorOverrideController overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = overrideController;
        overrideController["OneShot"] = clip;
        animator.Play("OneShot", 0, 0f);
        return clip.length;
    }

	// plays specified screen effect
	public void playScreenEffect(string name) {
		camera_animator.Play (name);
	}

	// playes specified flash effect
	public void playFlashEffect(string name) {
		flash_animator.Play (name);
	}
}
