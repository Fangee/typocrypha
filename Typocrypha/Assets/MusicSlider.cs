using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicSlider : MonoBehaviour {

    public Slider slider;

	// Use this for initialization
	void Start () {
        slider.onValueChanged.AddListener(delegate {ValueChangeCheck(); });
        slider.value = AudioPlayer.main.MusicVolume;
	}

    void ValueChangeCheck(){
        AudioPlayer.main.MusicVolume = slider.value;
    }
}
