using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Allows for setting color of sections of text
public class FXTextColor : FXTextEffect {
	public Color color; // color to set to

	public override void initEffect () {
		color = Color.black;
	}

	public override void stepEffect (UIVertex[] stream) {
		for (int i = 0; i < chars.Length; i+=2) {
			for (int j = chars[i]; j < chars[i+1]; ++j) {
				if (j >= stream.Length) return;
				int k = j * 6;
				for (int c = 0; c < 6; ++c)
					stream [k + c].color = color;
			}
		}
	}

	public override FXTextEffect clone (GameObject obj)
	{
		FXTextColor f = obj.AddComponent<FXTextColor>();
		f.color = color;
		return f;
	}

}
