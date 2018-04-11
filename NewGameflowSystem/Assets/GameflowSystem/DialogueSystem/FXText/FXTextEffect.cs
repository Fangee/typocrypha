using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Base class for special fx on 'FXText' components
// Note: every 6 entries in the stream array is a single quad (two triangles) for one character
public class FXTextEffect : MonoBehaviour {
	// which characters to apply effect to
	// each pair is a range, inclusive lower, exclusive upper
	public int[] chars;
	// font of text
	public Font font;
	// initializes effect w/ defaults
	public virtual void initEffect () {}
	// transforms vertices one step
	public virtual void stepEffect (UIVertex[] stream) {}
	// end effect
	public virtual void stopEffect (UIVertex[] stream) {}
	// clones effect, and attaches it to game object
	public virtual FXTextEffect clone(GameObject obj) {return null;}
}
