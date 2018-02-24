using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// post processing effects that makes screen wavy
public class PostProcess : MonoBehaviour {
	public static PostProcess main = null;
	public Material mat;

	public string[] shader_props; // shader property names
	public float[] shader_params; // shader property parameters

	void Awake() {
		if (main == null) main = this;
	}

	void Update() {
		for (int i = 0; i < shader_props.Length; ++i)
			mat.SetFloat (shader_props[i], shader_params[i]);
	}

	void OnRenderImage(RenderTexture src, RenderTexture dest) {
		Graphics.Blit (src, dest, mat);
	}
}
