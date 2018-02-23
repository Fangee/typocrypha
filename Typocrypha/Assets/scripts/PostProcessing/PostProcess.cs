using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// post processing effects that makes screen wavy
public class PostProcess : MonoBehaviour {
	public static PostProcess main = null;
	public Material mat;

	public float pixel_size; // size of pixel in pixelate effect

	void Awake() {
		if (main == null) main = this;
	}

	void Update() {
		mat.SetFloat ("_PixelSize", pixel_size);
	}

	void OnRenderImage(RenderTexture src, RenderTexture dest) {
		Graphics.Blit (src, dest, mat);
	}
}
