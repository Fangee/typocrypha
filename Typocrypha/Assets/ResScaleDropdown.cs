using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResScaleDropdown : MonoBehaviour {

    public GameObject cam;
    SetResolution sr;
    Dropdown resdrop;

	// Use this for initialization
	void Start () {

        sr = cam.GetComponent<SetResolution>();
        resdrop = GetComponent<Dropdown>();

        resdrop.onValueChanged.AddListener(delegate{ DropdownValueChanged(resdrop); });
	}

    void DropdownValueChanged(Dropdown d){
        switch (d.value)
        {
            case 0:
                sr.SetRes(1280, 720);
                break;
            case 1:
                sr.SetRes(1152, 648);
                break;
            case 2:
                sr.SetRes(1024, 576);
                break;
            case 3:
                sr.SetRes(960, 540);
                break;
            case 4:
                sr.SetRes(896, 504);
                break;
            case 5:
                sr.SetRes(800, 450);
                break;
            case 6:
                sr.SetRes(768, 432);
                break;
            case 7:
                sr.SetRes(640, 360);
                break;
        }
    }
	
}
