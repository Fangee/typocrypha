using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
// CHANNEL GUIDE:
//   SFX can be played in one of 8 channels (numbered 0-7).
//   playSFX(string) will just choose the first one that's available, so for general use, use that
//   Currently reserved channels:
//     3: talking effects
//     4: spell sfx

//// different types of music (DEPRECATED)
//public enum MusicType { CUTSCENE, BATTLE };

//// different types of sfx (DEPRECATED)
//public enum SFXType { UI, SPELL, BATTLE, SPEAKING, OTHER, size };

// manages playing music and sfx
public class AudioPlayer : MonoBehaviour {
	public static AudioPlayer main = null; // static global ref
	public bool ready; // are all the assets loaded?
	public AudioSource music; // plays music
	public Transform sfx; // contains sfx channels
	AudioSource[] sfx_channels; // sfx channels
	bool[] channel_reserved; // is that channel reserved?
	AssetBundle sfx_bundle;
	AssetBundle music_bundle;

    public const int channel_voice = 3;
    public const int channel_spell_sfx = 4;

    // music volume (0 to 1)
    public float MusicVolume {
        get { return music.volume; }
        set { music.volume = Mathf.Clamp01(value); }
    }

    //SFX volume (-1 to 0.0) 
    private float sfx_volume = 1.0F;
    public float SfxVolume {
        get { return sfx_volume; }
        set { sfx_volume = Mathf.Clamp01(value); }
    }

    //Speech Volume (0 to 1)
    public float VoiceVolume {
        get { return sfx_channels[channel_voice].volume; }
        set { sfx_channels[channel_voice].volume = Mathf.Clamp01(value); }
    }

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
		channel_reserved = new bool[sfx.childCount];
		for (int i = 0; i < sfx.childCount; i++)  // put all sfx channels into array
			sfx_channels [i] = sfx.GetChild (i).gameObject.GetComponent<AudioSource> ();
		initReservations ();
		ready = true;
		Debug.Log ("finished loading audio");
	}
    //Currently has the Audio-adjustment keybinds (will remove when we get a settings GUI)
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad8))
            MusicVolume += 0.1F;
        else if (Input.GetKeyDown(KeyCode.Keypad2))
            MusicVolume -= 0.1F;
        if (Input.GetKeyDown(KeyCode.Keypad9))
            SfxVolume += 0.1F;
        else if (Input.GetKeyDown(KeyCode.Keypad3))
            SfxVolume -= 0.1F;
        if (Input.GetKeyDown(KeyCode.Keypad7))
            VoiceVolume += 0.1F;
        else if (Input.GetKeyDown(KeyCode.Keypad1))
            VoiceVolume -= 0.1F;
    }

    // sets preset reservations
    void initReservations() {
		channel_reserved [channel_voice] = true; // reserved for talking sfx
        channel_reserved [channel_spell_sfx] = true;
		AudioPlayer.main.setSFX (channel_voice, "speak_boop"); // put default talk sfx in channel 3
	}

	// sets specified sfx channel
	public void setSFX(int channel, string name) {
		name = name.Trim ();
		if (name.CompareTo ("_") == 0) // skip if null song
			return; 
		if (name.CompareTo ("") == 0) { // set to no song
			sfx_channels[channel].Stop();
			sfx_channels[channel].clip = null;
			return;
		}
		sfx_channels [channel].clip = sfx_bundle.LoadAsset<AudioClip> (name);
	}

	// play current sfx in channel
	public void playSFX(int channel) {
		sfx_channels [channel].Play ();
	}

    // play sfx from name with specified volume modifier (finds first open channel)
    public void playSFX(string name, float volume = 1.0F)
    {
        for (int i = 0; i < sfx_channels.Length; ++i)
        {
            AudioSource channel = sfx_channels[i];
            if (!channel_reserved[i] && !channel.isPlaying)
            {
                channel.PlayOneShot(sfx_bundle.LoadAsset<AudioClip>(name), volume * SfxVolume);
                break;
            }
        }
    }

    // play music from name
    public void playMusic(string name) {
		name = name.Trim ();
		if (name.CompareTo ("_") == 0) // skip if null song
			return; 
		if (name.CompareTo ("") == 0) { // set to no song
			music.Stop();
			music.clip = null;
			return;
		}
		if (name.CompareTo ("STOP") == 0) { // stop if stop flag
			stopAll();
			return;
		} 
		music.clip = music_bundle.LoadAsset<AudioClip> (name);
		music.Play ();
	}

	// stop all audio clips
	public void stopAll() {
		music.Stop ();
		foreach (Transform child in sfx)
			child.gameObject.GetComponent<AudioSource> ().Stop();
	}

    // pause sfx
    public void pauseSFX() {
        foreach (Transform child in sfx)
            child.gameObject.GetComponent<AudioSource> ().Pause();
        music.UnPause();
    }

    // unpause sfx
    public void unpauseSFX() {
        foreach (Transform child in sfx)
            child.gameObject.GetComponent<AudioSource> ().UnPause();
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

/**************** DEPRECATED AUDIO SYSTEM *********************/

	//// sets specified sfx channel (DEPRECATED)
	//public void setSFX(int channel, SFXType type, string name) {
	//	name = name.Trim ();
	//	if (name.CompareTo ("null") == 0) return;
	//	sfx_channels [channel].clip = sfx_bundle.LoadAsset<AudioClip> (name);
	//}

	//// load and play sfx directly (DEPRECATED)
	//public void playSFX(int channel, SFXType type, string name) {
	//	setSFX (channel, type, name);
	//	sfx_channels [channel].Play ();
	//}

	//// play music from specified type (DEPRECATED)
	//public void playMusic(MusicType type, string name) {
	//	if (name.CompareTo ("_") == 0 || name.CompareTo ("") == 0) return; // skip if null song
 //       else if (name.CompareTo ("STOP") == 0)
 //       {
 //           stopAll();
 //           return;
 //       } // stop if stop flag
	//	music.clip = music_bundle.LoadAsset<AudioClip> (name);
	//	music.Play ();
	//}
}
