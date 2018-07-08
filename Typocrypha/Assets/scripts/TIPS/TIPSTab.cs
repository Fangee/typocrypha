using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base class for TIPS tabs
public abstract class TIPSTab : MonoBehaviour {

    // Open/close TIPS tab
    public abstract bool isOpen
    {
        get;
        set;
    }
}
