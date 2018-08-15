﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
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
public class AudioPlayer : MonoBehaviour
{
    public static AudioPlayer main = null; // static global ref
    public bool ready; // are all the assets loaded?
    public AudioSource music; // plays music
    public Transform sfx; // contains sfx channels
    public AudioMixerSnapshot bgm_full; // snapshot for full volume music
    public AudioMixerSnapshot bgm_off; // snapshot for no music

    AudioSource[] sfx_channels; // sfx channels
    bool[] channel_reserved; // is that channel reserved?
    AssetBundle sfx_bundle;
    AssetBundle music_bundle;

    public const int channel_voice = 3;
    public const int channel_spell_sfx = 4;

    // music volume (0 to 1)
    public float MusicVolume
    {
        get { return music.volume; }
        set { music.volume = Mathf.Clamp01(value); }
    }

    //SFX volume (-1 to 0.0) 
    private float sfx_volume = 1.0F;
    public float SfxVolume
    {
        get { return sfx_volume; }
        set { sfx_volume = Mathf.Clamp01(value); }
    }

    //Speech Volume (0 to 1)
    public float VoiceVolume
    {
        get { return sfx_channels[channel_voice].volume; }
        set { sfx_channels[channel_voice].volume = Mathf.Clamp01(value); }
    }

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        if (main == null) main = this;
        else GameObject.Destroy(gameObject); // avoid multiple copies
        ready = false;
    }

    void Start()
    {
        sfx_bundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(Application.streamingAssetsPath, "sfx"));
        music_bundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(Application.streamingAssetsPath, "music"));
        sfx_channels = new AudioSource[sfx.childCount];
        channel_reserved = new bool[sfx.childCount];
        for (int i = 0; i < sfx.childCount; i++)  // put all sfx channels into array
            sfx_channels[i] = sfx.GetChild(i).gameObject.GetComponent<AudioSource>();
        initReservations();
        ready = true;
        Debug.Log("finished loading audio");
    }

    // sets preset reservations
    void initReservations()
    {
        channel_reserved[channel_voice] = true; // reserved for talking sfx
        channel_reserved[channel_spell_sfx] = true;
        AudioPlayer.main.setSFX(channel_voice, "speak_boop"); // put default talk sfx in channel 3
    }

    // sets specified sfx channel
    public void setSFX(int channel, string name)
    {
        name = name.Trim();
        if (name.CompareTo("_") == 0) // skip if null song
            return;
        if (name.CompareTo("") == 0)
        { // set to no song
            sfx_channels[channel].Stop();
            sfx_channels[channel].clip = null;
            return;
        }
        sfx_channels[channel].clip = sfx_bundle.LoadAsset<AudioClip>(name);
    }

    // sets specified sfx channel
    public void setSFX(int channel, AudioClip sfx)
    {
        sfx_channels[channel].clip = sfx;
    }

    // sets the voice sfx channel (text scroll sfx)
    public void setVoiceSFX(AudioClip sfx)
    {
        setSFX(channel_voice, sfx);
    }

	// play current sfx in channel
	public void playSFX(int channel) {
		float volume = 1.0F;
		sfx_channels [channel].PlayOneShot(sfx_channels [channel].clip, volume * SfxVolume);
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

    //Get the current pitch of the specified channel
    public float getPitch(int channel)
    {
        return sfx_channels[channel].pitch;
    }
    //Set the current pitch of the specified channel
    public void setPitch(int channel, float pitch)
    {
        sfx_channels[channel].pitch = pitch;
    }
    //Increment the pitch of the specified channel by arg: changeBy (can be negative)
    public void changePitch(int channel, float changeBy)
    {
        sfx_channels[channel].pitch += changeBy;
    }

    // play music from name
    public void playMusic(string name) {
		name = name.Trim ();
		StartCoroutine (loadAndPlay (name));
	}

	// load sound asynchronously, and play when ready
	IEnumerator loadAndPlay(string name) {
		AssetBundleRequest request = music_bundle.LoadAssetAsync<AudioClip> (name);
		yield return new WaitUntil (() => request.isDone);
		music.clip = (AudioClip)request.asset;
		music.Play ();
	}

	// stop music
	public void stopMusic() {
		music.Stop ();
	}

	// stop all audio clips
	public void stopAll() {
		stopMusic ();
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

	// fade music in/out linearly over time (dir = false: in, dir = true: out)
	public void fadeMusic(bool dir, float time) {
		if (dir)
			bgm_off.TransitionTo (time);
		else
			bgm_full.TransitionTo (time);
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
