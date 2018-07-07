using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Manages searching through TIPS database
public class TIPSSearch : MonoBehaviour {
    public InputField search_bar;

    // Open/close TIPS search tab
    public bool isOpen
    {
        // Get whether TIPS search tab is open or not
        get
        {
            return gameObject.activeInHierarchy;
        }
        // Open/close TIPS search tab (true/false respectively)
        set
        {
            Debug.Log("open TIPS search:" + value);
            gameObject.SetActive(value);
            if (value)
            {
                Debug.Log("activate");
                search_bar.ActivateInputField();
            }
            else
            {
                Debug.Log("deactivate");
                search_bar.DeactivateInputField();
            }
        }
    }

    // Called when text is submitted to search bar
    public void submit()
    {
        Debug.Log("search:" + search_bar.text);
    }
}
