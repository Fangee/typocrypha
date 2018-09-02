using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Text class, but with functionality for special effects
public class FXText : Text {
	public UIVertex[] stream_arr; // array of vertex triangles
	public bool is_rendered; // has the text been rendered yet?

	List<FXTextEffect> effect_list = new List<FXTextEffect>(); // list of effects being applied
	List<UIVertex> stream = new List<UIVertex>(); // vertex triangle stream

	protected override void Start() {
		base.Start ();
		/* TESTING */
		/*
		FXTextShake shake = gameObject.AddComponent<FXTextShake> ();
		shake.intensity = 2f;
		shake.chars = new int[2] {0,6};
		shake.initEffect (stream_arr);
		effect_list.Add (shake);

		FXTextWave wave = gameObject.AddComponent<FXTextWave> ();
		wave.amplitude = 5f;
		wave.wavelength = 0.3f;
		wave.frequency = 0.3f;
		wave.chars = new int[4] {4,10,14,18};
		wave.initEffect (stream_arr);
		effect_list.Add (wave);

		FXTextColorCycle color_cycle = gameObject.AddComponent<FXTextColorCycle> ();
		color_cycle.color_count = 16;
		color_cycle.period = 0.1f;
		color_cycle.chars = new int[2] { 6, 14 };
		color_cycle.initEffect (stream_arr);
		effect_list.Add (color_cycle);

		FXTextScramble scramble = gameObject.AddComponent<FXTextScramble> ();
		scramble.font = font;
		scramble.period = 0.05f;
		scramble.chars = new int[2] { 4, 10 };
		scramble.initEffect (stream_arr);
		effect_list.Add (scramble);

		FXTextPulse pulse = gameObject.AddComponent<FXTextPulse> ();
		pulse.amplitude = 3f;
		pulse.wavelength = 2f;
		pulse.frequency = 3f;
		pulse.chars = new int[2] { 12, 20 };
		pulse.initEffect (stream_arr);
		effect_list.Add (pulse);
		*/
	}

	protected override void OnPopulateMesh (VertexHelper toFill)
	{
		base.OnPopulateMesh (toFill);
		if (effect_list.Count > 0) {
			toFill.GetUIVertexStream (stream);
			stream_arr = stream.ToArray ();
			foreach (FXTextEffect effect in effect_list) effect.stepEffect (stream_arr);
			toFill.Clear ();
			toFill.AddUIVertexTriangleStream (new List<UIVertex> (stream_arr));
		}
		is_rendered = true;
	}

	void Update() {
		if (effect_list.Count > 0) SetAllDirty (); // force update
	}

	// adds a new effect
	public void addEffect(FXTextEffect text_effect) {
		text_effect.font = font;
		effect_list.Add (text_effect);
	}

	// removes all currently applied effects
	public void clearEffects() {
		foreach (FXTextEffect effect in effect_list) {
			effect.stopEffect (stream_arr);
			effect_list.Remove (effect);
			Destroy (effect);
		}
		SetAllDirty ();
	}

	// removes specific effect
	public void removeEffect(FXTextEffect fx) {
		effect_list.Remove (fx);
	}
}
