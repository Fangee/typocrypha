using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogItemVN : DialogItem
{
    public string speakerName;
    public Sprite mcSprite; // Sprite of main character (displayed in text box)
    public Sprite codecSprite; // Sprite of main character (displayed in text box)
    public DialogItemVN(string text, string speakerName, Sprite mcSprite, Sprite codecSprite) : base(text)
    {
        this.speakerName = speakerName;
        this.mcSprite = mcSprite;
        this.codecSprite = codecSprite;
    }
}

