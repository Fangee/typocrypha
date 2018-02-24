using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// screen pixelation post processing effect
public class PixelateScreen : PostProcess {
	public float pixel_size;

	void Start() {
		shader_props [0] = "_PixelSize";
	}

	void Update() {
		shader_params [0] = pixel_size;
		for (int i = 0; i < shader_props.Length; ++i)
			mat.SetFloat (shader_props[i], shader_params[i]);
	}
}
