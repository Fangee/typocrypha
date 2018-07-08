using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Manages TIPS tab buttons (on the sidebar)
public class TIPSTabButton : MonoBehaviour {
    public TIPSTab tab; // Tab this button is linked to
    public Image image; // Image component of button
    public Color highlight_color; // Highlight tint color

    bool _isHighlight; // Is button highlighted?
    public bool isHighlight
    {
        // Check whether button is highlighted or not
        get
        {
            return _isHighlight;
        }
        // Turn highlight on/off
        set
        {
            _isHighlight = value;
            if (_isHighlight)
            {
                image.color = highlight_color;
            }
            else
            {
                image.color = Color.white;
            }
        }
    }

    void Awake()
    {
        isHighlight = false;
    }
}
