using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Text pulsing (expanding/contracting) effect
public class FXTextPulse : FXTextEffect {
	public float amplitude = 1f; // size of pulse
	public float wavelength = 1f; // length of pulse
	public float frequency = 1f; // inverse of period

	float time = 0f;
	Vector3 upleft = new Vector3 (-1, 1, 0);
	Vector3 upright = new Vector3 (1, 1, 0);
	Vector3 downleft = new Vector3 (-1, -1, 0);
	Vector3 downright = new Vector3 (1, -1, 0);

	public override void initEffect () {
		amplitude = 3f;
		wavelength = 2f;
		frequency = 3f;
		time = 0f;
	}

	public override void stepEffect (UIVertex[] stream) {
		for (int i = 0; i < chars.Length; i+=2) {
			for (int j = chars[i]; j < chars[i+1]; ++j) {
				float amt = Mathf.Sin ((time + (j/wavelength)) * frequency) * amplitude;
				int k = j * 6;
				stream [k].position += upleft * amt;
				stream [k + 1].position += upright * amt;
				stream [k + 2].position += downright * amt;
				stream [k + 3].position += downright * amt;
				stream [k + 4].position += downleft * amt;
				stream [k + 5].position += upleft * amt;
			}
		}
		time += Time.deltaTime;
	}

	public override FXTextEffect clone (GameObject obj)
	{
		FXTextPulse f = obj.AddComponent<FXTextPulse>();
		f.amplitude = amplitude;
		f.wavelength = wavelength;
		f.frequency = frequency;
		return f;
	}

}
