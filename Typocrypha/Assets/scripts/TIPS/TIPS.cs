using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Top-level controller class for TIPS interface
// Manages tab selection and changing
public class TIPS : MonoBehaviour {
    public GameObject TIPSUI; // Object containing UI for TIPS interface
    public TIPSSearch tips_search; // Search tab controller

    void Update()
    {
        // Toggle TIPS menu on 'TAB'
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isOpen = !isOpen;
        }
    }

    // Open/close TIPS menu
    public bool isOpen 
    {
        // Get whether TIPS menu is open or not
        get
        {
            return TIPSUI.activeInHierarchy;
        }
        // Open/close TIPS menu (true/false respectively)
        set
        {
            Debug.Log("open TIPS:" + value);
            Time.timeScale = 0;
            TIPSUI.SetActive(value);
            tips_search.isOpen = value; //TEMP: should open last opened tab
        }
    }
	
}
