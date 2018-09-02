using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Text sin wave effect
public class FXTextWave : FXTextEffect {
	public float amplitude = 1f; // height of wave
	public float wavelength = 1f; // length of wave
	public float frequency = 1f; // inverse of period

	float time = 0f; // internal counter

	public override void initEffect () {
		amplitude = 5f;
		wavelength = 0.3f;
		frequency = 0.3f;
		time = 0f;
	}

	public override void stepEffect (UIVertex[] stream) {
		for (int i = 0; i < chars.Length; i+=2) {
			for (int j = chars[i]; j < chars[i+1]; ++j) {
				float wave_amt = Mathf.Sin ((time+(j/wavelength))*frequency) * amplitude;
				int k = j * 6;
				for (int c = 0; c < 6; ++c)
					stream [k+c].position.y += wave_amt;
			}
		}
		++time;
	}

	public override FXTextEffect clone (GameObject obj)
	{
		FXTextWave f = obj.AddComponent<FXTextWave>();
		f.amplitude = amplitude;
		f.wavelength = wavelength;
		f.frequency = frequency;
		return f;
	}
}
