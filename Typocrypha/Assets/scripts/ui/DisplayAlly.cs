using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayAlly : MonoBehaviour {

    public GameObject bar_prefab;
    
    private Ally ally = null;
    private BarMeter gauge;
    private bool stun = false;
    private bool hurt = false;
    public Text ready_text;
    public Text name_text;

    public void setAlly(Ally a)
    {
        if(a != null)
        {
            ally = a;
            GameObject new_bar = GameObject.Instantiate(bar_prefab, transform);
            new_bar.transform.localScale = new Vector3(1, 1, 1);
            new_bar.transform.position = transform.position;
            gauge = new_bar.GetComponent<BarMeter>();
            name_text.text = a.Stats.name; 
        }

    }

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update ()
    {

        if (ally != null)
        {
            float percent = ally.getPercent();
            if(ally.Is_stunned == true)
            {
                ready_text.text = "STUNNED";
                if(stun == false)
                {
                    gauge.gameObject.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 0.5F, 0);
                    stun = true;
                }

            }
            else if(percent < 0.5F)
            {
                ready_text.text = "";
                if(!hurt || stun)
                {
                    gauge.gameObject.transform.GetChild(0).GetComponent<Image>().color = new Color(0.9F, 0, 0);
                    stun = false;
                    hurt = true;
                }

            }
            else if (percent >= 1F)
            {
                ready_text.text = "SUPER READY";
                if(hurt || stun)
                {
                    gauge.gameObject.transform.GetChild(0).GetComponent<Image>().color = new Color(0, 0.9F, 0);
                    hurt = false;
                }
            }
            else if (percent >= 0.75F)
            {
                ready_text.text = "ASSIST READY";
                if (hurt || stun)
                {
                    gauge.gameObject.transform.GetChild(0).GetComponent<Image>().color = new Color(0, 0.9F, 0);
                    hurt = false;
                }
            }
            else
            {
                ready_text.text = "";
                if (hurt || stun)
                {
                    gauge.gameObject.transform.GetChild(0).GetComponent<Image>().color = new Color(0, 0.9F, 0);
                    hurt = false;
                }
            }
            gauge.setValue(percent);
        }

    }
}
