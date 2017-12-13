using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteComparer : IComparer {
	public int Compare(object a, object b) {
		Sprite sa = (Sprite)a;
		Sprite sb = (Sprite)b;
		return sa.name.CompareTo (sb.name);
	}
}

// different types of animations
public enum AnimationType { SPELL };

// plays effect animations
public class AnimationPlayer : MonoBehaviour {
	public static AnimationPlayer main = null; // global static ref
	public Transform display_pos; // positions display
	public SpriteRenderer display_sprite; // renders display
	public int anim_frames; // number of game frames per animation frame
	SpriteComparer sprite_comparer; // compares sprites
	AssetBundle spellanim; // spell animations bundle

	void Awake() {
		if (main == null) main = this;
		sprite_comparer = new SpriteComparer ();
	}

	void Start() {
		spellanim = AssetBundle.LoadFromFile (System.IO.Path.Combine(Application.streamingAssetsPath, "spellanim"));
	}

	// plays specified animation
	public void playAnimation(AnimationType type, string name, Vector2 pos, bool loop) {
		display_pos.position = pos;
		switch (type) {
		case AnimationType.SPELL:
			StartCoroutine (draw(spellanim.LoadAsset<SpriteAtlas>(name), loop));
			break;
		}
	}

	// draws animation frame by frame
	IEnumerator draw(SpriteAtlas atlas, bool loop) {
		Sprite[] sprites = new Sprite[atlas.spriteCount];
		atlas.GetSprites (sprites);
		System.Array.Sort (sprites, sprite_comparer); // resort sprites
		do {
			foreach (Sprite sprite in sprites) {
				display_sprite.sprite = sprite;
				for (int i = 0; i < anim_frames; ++i)
					yield return new WaitForEndOfFrame();
			}
		} while(loop);
		display_sprite.sprite = null;
	}
}
