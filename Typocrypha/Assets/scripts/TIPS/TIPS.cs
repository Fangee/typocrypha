using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Top-level controller class for TIPS interface
// Manages tab selection and changing
public class TIPS : MonoBehaviour {
    public GameObject TIPSUI; // Object containing UI for TIPS interface
    public Transform TIPSTabButtons; // Object transform containing all tab button objects as children
    public Transform TIPSTabs; // Object transform containing all tab objects

    TIPSTabButton curr_tab_button; // Currently selected tab button

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
            pause();
            // TRANSITION CODE
            TIPSUI.SetActive(value);
            openTab(curr_tab_button);
        }
    }

    void Awake()
    {
        // Initialize current tab to first tab
        curr_tab_button = TIPSTabButtons.GetChild(0).GetComponent<TIPSTabButton>();
    }

    void Update()
    {
        // Toggle TIPS menu when TAB key is pressed
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isOpen = !isOpen;
        }
        // Select different tabs with UP/DOWN arrow keys
        int ind_inc = 0;
        if (Input.GetKeyDown(KeyCode.UpArrow))
            ind_inc = -1;
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ind_inc = 1;
        if (ind_inc != 0)
        {
            int curr_ind = curr_tab_button.transform.GetSiblingIndex();
            int next_ind = curr_ind + ind_inc;
            if (next_ind < 0 || next_ind >= TIPSTabButtons.childCount)
            {
                // BAD TAB SELECTION FX
                next_ind = Mathf.Clamp(curr_ind + ind_inc, 0, TIPSTabButtons.childCount - 1);
            }
            openTab(TIPSTabButtons.GetChild(next_ind).GetComponent<TIPSTabButton>());
        }
    }

    // Pauses game for access to TIPS
    void pause()
    {
        Time.timeScale = 0;
        // PAUSE CODE HERE
    }

    // Opens selected tab from tab button
	void openTab(TIPSTabButton tab_button)
    {
        // Close old current tab
        curr_tab_button.isHighlight = false;
        curr_tab_button.tab.isOpen = false;
        // Set new current tab and open it
        curr_tab_button = tab_button;
        curr_tab_button.isHighlight = true;
        curr_tab_button.tab.isOpen = true;
    }
}
