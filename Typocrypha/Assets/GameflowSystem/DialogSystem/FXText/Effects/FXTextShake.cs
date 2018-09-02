using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Text shaking effect
public class FXTextShake : FXTextEffect {
	public float intensity = 1f; // intensity of shaking

	public override void initEffect () {
		intensity = 2f;
	}

	public override void stepEffect (UIVertex[] stream) {
		for (int i = 0; i < chars.Length; i+=2) {
			for (int j = chars[i]; j < chars[i+1]; ++j) {
				Vector3 shake_amt = Random.insideUnitSphere * intensity;
				int k = j * 6;
				for (int c = 0; c < 6; ++c)
					stream [k+c].position += shake_amt;
			}
		}
	}

	public override FXTextEffect clone (GameObject obj)
	{
		FXTextShake f = obj.AddComponent<FXTextShake>();
		f.intensity = intensity;
		return f;
	}
}
