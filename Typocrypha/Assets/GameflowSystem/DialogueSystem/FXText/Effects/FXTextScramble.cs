using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Text random character scramble effect
public class FXTextScramble : FXTextEffect {
	public float period = 1f; // how long between character changes

	CharacterInfo[] char_info;
	int char_count;

	public override void initEffect () {
		period = 0.05f;
		char_info = font.characterInfo;
		char_count = char_info.Length;
		StartCoroutine (charSwitch ());
	}

	public override void stepEffect (UIVertex[] stream) {
		for (int i = 0; i < chars.Length; i+=2) {
			for (int j = chars[i]; j < chars[i+1]; ++j) {
				CharacterInfo rand_char = char_info[j % char_count];
				int k = j * 6;
				stream [k].uv0 = rand_char.uvTopLeft;
				stream [k+1].uv0 = rand_char.uvTopRight;
				stream [k+2].uv0 = rand_char.uvBottomRight;
				stream [k+3].uv0 = rand_char.uvBottomRight;
				stream [k+4].uv0 = rand_char.uvBottomLeft;
				stream [k+5].uv0 = rand_char.uvTopLeft;
			}
		}
	}

	public override void stopEffect (UIVertex[] stream) {
		StopAllCoroutines ();
	}

	// switches characters every period
	IEnumerator charSwitch() {
		for (;;) {
			// scramble char array
			for (int i = 0; i < char_count; ++i) {
				int j = (int)Random.Range (i, char_count);
				CharacterInfo tmp = char_info [i];
				char_info [i] = char_info [j];
				char_info [j] = tmp;
			}
			yield return new WaitForSeconds (period);
		}
	}

	public override FXTextEffect clone (GameObject obj)
	{
		FXTextScramble f = obj.AddComponent<FXTextScramble>();
		f.period = period;
		return f;
	}
}
