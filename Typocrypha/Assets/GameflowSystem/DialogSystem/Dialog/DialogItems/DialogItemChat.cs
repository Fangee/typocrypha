using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogItemChat : DialogItem
{
    public string speakerName;
    public Sprite leftIcon; // Icon for left speaker icon (CHAT MODE) 
    public Sprite rightIcon; // Icon for right speaker icon (CHAT MODE)
    public IconSide iconSide; // Side where icon shows (CHAT MODE)
    public DialogItemChat(string text, string speakerName, IconSide iconSide, Sprite leftIcon = null, Sprite rightIcon = null) : base(text)
    {
        this.speakerName = speakerName;
        this.iconSide = iconSide;
        this.leftIcon = leftIcon;
        this.rightIcon = rightIcon;
    }
}
