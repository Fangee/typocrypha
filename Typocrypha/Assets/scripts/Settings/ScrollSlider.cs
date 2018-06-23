using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollSlider : MonoBehaviour {

    public Slider slider;

    // Use this for initialization
    void Start () {
        slider.onValueChanged.AddListener(delegate {ValueChangeCheck(); });
        slider.value = DialogueManager.main.scroll_scale;
    }

    void ValueChangeCheck(){
		DialogueManager.main.scroll_scale = 0.1f+(11-(slider.value*10));
    }
}
