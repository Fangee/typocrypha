using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

// compares sprites by name (for sorting animation frames from atlus)
public class SpriteComparer : IComparer {
	public int Compare(object a, object b) {
		Sprite sa = (Sprite)a;
		Sprite sb = (Sprite)b;
		return sa.name.CompareTo (sb.name);
	}
}
	
// different types of animations (DEPRECATED)
public enum AnimationType { SPELL };

// plays effect animations
public class AnimationPlayer : MonoBehaviour {
	public static AnimationPlayer main = null; // global static ref
	public bool ready; // are all of the assets loaded?
	public GameObject animation_holder_prefab; // object prefab that holds the animations

	SpriteComparer sprite_comparer = null; // compares sprites (DEPRECATED)
	AssetBundle anim = null; // spell animations bundle (DEPRECATED)
	float anim_framelen = 0; // length of an animation frame (DEPRECATED)

	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
		if (main == null) main = this;
		else GameObject.Destroy (gameObject); // avoid multiple copies
		ready = true;
		/* DEPRECATED
		sprite_comparer = new SpriteComparer ();
		ready = false;
		*/
	}

	void Start() {
		/* DEPRECATED (animations now stored as animation clips within an Animator)
		anim = AssetBundle.LoadFromFile (System.IO.Path.Combine(Application.streamingAssetsPath, "animations"));
		ready = true;
		Debug.Log ("finished loading animations");
		*/
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

	// DEPRECATED
	// draws animation frame by frame
	IEnumerator draw(SpriteAtlas atlas, int loop, SpriteRenderer display_sprite) {
		Sprite[] sprites = new Sprite[atlas.spriteCount];
		atlas.GetSprites (sprites);
		System.Array.Sort (sprites, sprite_comparer); // resort sprites from atlas
		do {
			foreach (Sprite sprite in sprites) {
				display_sprite.sprite = sprite;
				yield return new WaitForSeconds(anim_framelen);
			}
		} while(--loop > 0);
		GameObject.Destroy (display_sprite.gameObject);
	}

	// DEPRECATED
	// plays specified animation; returns coroutine to keep track of animation's progress
	// animation will loop 'loop' times (will always run at least once)
	public Coroutine playAnimation(string name, Vector2 pos, int loop) {
		GameObject display = new GameObject (); // make a new animation sprite holder
		display.transform.SetParent (transform);
		display.transform.position = pos;
		SpriteRenderer sprite_r = display.AddComponent<SpriteRenderer> ();
		sprite_r.sortingOrder = 15; // put animation on top
		return StartCoroutine (draw(anim.LoadAsset<SpriteAtlas>(name), loop, sprite_r));
	}

	// DEPRECATED
	// plays specified animation; returns coroutine to keep track of animation's progress
	// animation will loop 'loop' times (will always run at least once)
	public Coroutine playAnimation(AnimationType type, string name, Vector2 pos, int loop) {
		GameObject display = new GameObject (); // make a new animation sprite holder
		display.transform.SetParent (transform);
		display.transform.position = pos;
		SpriteRenderer sprite_r = display.AddComponent<SpriteRenderer> ();
		sprite_r.sortingOrder = 15; // put animation on top
		return StartCoroutine (draw(anim.LoadAsset<SpriteAtlas>(name), loop, sprite_r));
	}
}
