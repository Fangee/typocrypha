using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// different types of music
public enum MusicType { CUTSCENE, BATTLE };

// SFX CHANNELS: (can be changed)
//   0 - menu/UI sounds
//   1 - spell cast sounds
//   2 - battle sounds (taking damage, etc)
//   3 - speaking sounds

// manages playing music and sfx
public class AudioPlayer : MonoBehaviour {
	public static AudioPlayer main = null; // static global ref
	public AudioSource music; // plays music
	public Transform sfx; // contains sfx channels
	AudioSource[] sfx_channels; // sfx channels
	AssetBundle cutscenemusic;
	AssetBundle battlemusic;
	AssetBundle speakingsfx;
	// ... add more asset bundles for diff audio assets here

	void Awake() {
		if (main == null) main = this;
		cutscenemusic = AssetBundle.LoadFromFile ("Assets/AssetBundles/cutscenemusic");
		battlemusic = AssetBundle.LoadFromFile ("Assets/AssetBundles/battlemusic");
		speakingsfx = AssetBundle.LoadFromFile ("Assets/AssetBundles/speakingsfx");
	}

	void Start() {
		sfx_channels = new AudioSource[sfx.childCount];
		for (int i = 0; i < sfx.childCount; i++) // put all sfx channels into array
			sfx_channels[i] = sfx.GetChild(i).gameObject.GetComponent<AudioSource>();
	}

	// sets specified speaking sfx
	public void setSpeakingSFX(string name) {
		sfx_channels [3].clip = speakingsfx.LoadAsset<AudioClip> (name);
	}

	// play speaking sfx current
	public void playSpeakingSFX() {
		sfx_channels [3].Play ();
	}

	// play music from specified type
	public void playMusic(MusicType type, string name) {
		switch (type) {
		case MusicType.CUTSCENE:
			music.clip = cutscenemusic.LoadAsset<AudioClip> (name);
			music.Play ();
			break;
		case MusicType.BATTLE:
			music.clip = battlemusic.LoadAsset<AudioClip> (name);
			music.Play ();
			break;
		}
	}

	// stop all audio clips
	public void stopAll() {
		music.Stop ();
		foreach (Transform child in sfx)
			child.gameObject.GetComponent<AudioSource> ().Stop();
	}

	// fade a sound clip to a stop
	IEnumerator fadeToStop(AudioSource source) {
		if (source.isPlaying) {
			while (source.volume > 0) {
				source.volume -= 0.05F;
				yield return new WaitForEndOfFrame ();
			}
		}
	}
}
