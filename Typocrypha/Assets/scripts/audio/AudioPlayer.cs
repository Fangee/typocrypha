using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// different types of music
public enum MusicType { CUTSCENE, BATTLE };

// different types of sfx
public enum SFXType { UI, SPELL, BATTLE, SPEAKING };

// manages playing music and sfx
public class AudioPlayer : MonoBehaviour {
	public static AudioPlayer main = null; // static global ref
	public AudioSource music; // plays music
	public Transform sfx; // contains sfx channels
	AudioSource[] sfx_channels; // sfx channels
	AssetBundle cutscenemusic;
	AssetBundle battlemusic;
	AssetBundle uisfx;
	AssetBundle spellsfx;
	AssetBundle battlesfx;
	AssetBundle speakingsfx;
	// ... add more asset bundles for diff audio assets here

	void Awake() {
		if (main == null) main = this;
	}

	void Start() {
		cutscenemusic = AssetBundle.LoadFromFile (System.IO.Path.Combine(Application.streamingAssetsPath, "cutscenemusic"));
		battlemusic = AssetBundle.LoadFromFile (System.IO.Path.Combine(Application.streamingAssetsPath, "battlemusic"));
		uisfx = AssetBundle.LoadFromFile (System.IO.Path.Combine(Application.streamingAssetsPath, "uisfx"));
		spellsfx = AssetBundle.LoadFromFile (System.IO.Path.Combine(Application.streamingAssetsPath, "spellsfx"));
		battlesfx = AssetBundle.LoadFromFile (System.IO.Path.Combine(Application.streamingAssetsPath, "battlesfx"));
		speakingsfx = AssetBundle.LoadFromFile (System.IO.Path.Combine(Application.streamingAssetsPath, "speakingsfx"));
		sfx_channels = new AudioSource[sfx.childCount];
		for (int i = 0; i < sfx.childCount; i++) // put all sfx channels into array
			sfx_channels[i] = sfx.GetChild(i).gameObject.GetComponent<AudioSource>();
	}
		
	// sets specified sfx channel
	public void setSFX(int channel, SFXType type, string name) {
		name = name.Trim ();
		switch (type) {
		case SFXType.UI:
			sfx_channels [channel].clip = uisfx.LoadAsset<AudioClip> (name);
			break;
		case SFXType.SPELL:
			sfx_channels [channel].clip = spellsfx.LoadAsset<AudioClip> (name);
			break;
		case SFXType.BATTLE:
			sfx_channels [channel].clip = battlesfx.LoadAsset<AudioClip> (name);
			break;
		case SFXType.SPEAKING:
			sfx_channels [channel].clip = speakingsfx.LoadAsset<AudioClip> (name);
			break;
		}
	}

	// play current sfx in channel
	public void playSFX(int channel) {
		sfx_channels [channel].Play ();
	}

	// load and play sfx directly
	public void playSFX(int channel, SFXType type, string name) {
		setSFX (channel, type, name);
		sfx_channels [channel].Play ();
	}

	// play music from specified type
	public void playMusic(MusicType type, string name) {
		if (name.CompareTo ("_") == 0) return; // skip if null song
		Debug.Log ("playing music:" + name);
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
