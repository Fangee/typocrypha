using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Example TIPS tab
public class TIPSOther : TIPSTab {

    // Open/close TIPS tab
    public override bool isOpen
    {
        // Get whether TIPS tab is open or not
        get
        {
            return gameObject.activeInHierarchy;
        }
        // Open/close TIPS tab (true/false respectively)
        set
        {
            gameObject.SetActive(value);
        }
    }

}
