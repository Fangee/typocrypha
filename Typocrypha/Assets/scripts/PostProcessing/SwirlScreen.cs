using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// screen swirl post processing effect
public class SwirlScreen : PostProcess {
	public float swirl_amount;

	void Start() {
		shader_props [0] = "_SwirlAmount";
	}

	void Update() {
		shader_params [0] = swirl_amount;
		for (int i = 0; i < shader_props.Length; ++i)
			mat.SetFloat (shader_props[i], shader_params[i]);
	}
}
