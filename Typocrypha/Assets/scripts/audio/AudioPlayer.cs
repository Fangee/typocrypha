using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// different types of music (DEPRECATED)
public enum MusicType { CUTSCENE, BATTLE };

// different types of sfx (DEPRECATED)
public enum SFXType { UI, SPELL, BATTLE, SPEAKING, OTHER, size };

// manages playing music and sfx
public class AudioPlayer : MonoBehaviour {
	public static AudioPlayer main = null; // static global ref
	public bool ready; // are all the assets loaded?
	public AudioSource music; // plays music
	public Transform sfx; // contains sfx channels
	AudioSource[] sfx_channels; // sfx channels
	AssetBundle sfx_bundle;
	AssetBundle music_bundle;

	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
		if (main == null) main = this;
		else GameObject.Destroy (gameObject); // avoid multiple copies
		ready = false;
	}

	void Start() {
		sfx_bundle = AssetBundle.LoadFromFile (System.IO.Path.Combine(Application.streamingAssetsPath, "sfx"));
		music_bundle = AssetBundle.LoadFromFile (System.IO.Path.Combine(Application.streamingAssetsPath, "music"));
		sfx_channels = new AudioSource[sfx.childCount];
		for (int i = 0; i < sfx.childCount; i++) // put all sfx channels into array
			sfx_channels[i] = sfx.GetChild(i).gameObject.GetComponent<AudioSource>();
		ready = true;
		Debug.Log ("finished loading audio");
	}

	// sets specified sfx channel
	public void setSFX(int channel, string name) {
		name = name.Trim ();
		if (name.CompareTo ("null") == 0) return;
		sfx_channels [channel].clip = sfx_bundle.LoadAsset<AudioClip> (name);
	}

	// play current sfx in channel
	public void playSFX(int channel) {
		sfx_channels [channel].Play ();
	}

	// play sfx from name (finds first open channel)
	public void playSFX(string name) {
		foreach (AudioSource channel in sfx_channels) {
			if (!channel.isPlaying) {
				channel.clip = sfx_bundle.LoadAsset<AudioClip> (name);
				channel.Play ();
				break;
			}
		}
	}

	// play music from name
	public void playMusic(string name) {
		if (name.CompareTo ("_") == 0 || name.CompareTo ("") == 0) return; // skip if null song
		else if (name.CompareTo ("STOP") == 0)
		{
			stopAll();
			return;
		} // stop if stop flag
		music.clip = music_bundle.LoadAsset<AudioClip> (name);
		music.Play ();
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

/**************** DEPRECATED AUDIO SYSTEM **********************************/

	// sets specified sfx channel (DEPRECATED)
	public void setSFX(int channel, SFXType type, string name) {
		name = name.Trim ();
		if (name.CompareTo ("null") == 0) return;
		sfx_channels [channel].clip = sfx_bundle.LoadAsset<AudioClip> (name);
	}

	// load and play sfx directly (DEPRECATED)
	public void playSFX(int channel, SFXType type, string name) {
		setSFX (channel, type, name);
		sfx_channels [channel].Play ();
	}

	// play music from specified type (DEPRECATED)
	public void playMusic(MusicType type, string name) {
		if (name.CompareTo ("_") == 0 || name.CompareTo ("") == 0) return; // skip if null song
        else if (name.CompareTo ("STOP") == 0)
        {
            stopAll();
            return;
        } // stop if stop flag
		music.clip = music_bundle.LoadAsset<AudioClip> (name);
		music.Play ();
	}
}
