using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SfxSlider : MonoBehaviour {

    public Slider slider;

    // Use this for initialization
    void Start () {
        slider.onValueChanged.AddListener(delegate {ValueChangeCheck(); });
        slider.value = AudioPlayer.main.SfxVolume;
    }

    void ValueChangeCheck(){
        AudioPlayer.main.SfxVolume = slider.value;
    }
}
