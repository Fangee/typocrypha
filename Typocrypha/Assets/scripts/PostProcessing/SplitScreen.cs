using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitScreen : PostProcess {
	public float slope;
	public float cut_height;
	public float cut_size;

	void Start() {
		shader_props [0] = "_Slope";
		shader_props [1] = "_CutHeight";
		shader_props [2] = "_CutSize";
	}

	void Update() {
		shader_params [0] = slope;
		shader_params [1] = cut_height;
		shader_params [2] = cut_size;
		for (int i = 0; i < shader_props.Length; ++i)
			mat.SetFloat (shader_props[i], shader_params[i]);
		mat.SetFloat ("_Width", Screen.width);
		mat.SetFloat ("_Height", Screen.height);
	}
}
