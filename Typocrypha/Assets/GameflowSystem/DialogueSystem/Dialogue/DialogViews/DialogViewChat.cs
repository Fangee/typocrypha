using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogViewChat : DialogView
{
    public GameObject dialogBoxPrefab;
    public RectTransform ChatContent; // Content of chat scroll view (contains dialogue boxes)
    public Scrollbar scroll_bar; // Scroll bar of chat dialogue window
    public GameObject spacebar_icon_chat; // Spacebar icon CHAT view
    public Animator animator_spacebar_chat; // Spacebar icon key animator

    public override DialogBox newDialog(DialogItem data)
    {
        //if (d_item.GetType () == typeof(DialogueItemChat)) {
        //	DialogueItemChat c_item = (DialogueItemChat)d_item;
        //	// Set icon
        //	left_icon.sprite = c_item.left_icon;
        //	right_icon.sprite = c_item.right_icon;
        //	// Display appropriate icon
        //	if (c_item.icon_side == IconSide.LEFT || c_item.icon_side == IconSide.BOTH)
        //		left_icon.enabled = true;
        //	if (c_item.icon_side == IconSide.RIGHT || c_item.icon_side == IconSide.BOTH)
        //		right_icon.enabled = true;
        //	// Add text with speaker's name, and offset text display
        //	int offset = 0;
        //	if (d_item.speaker_name != null && d_item.speaker_name.Length != 0) {
        //		text = d_item.speaker_name + "\n" + text;
        //		offset += d_item.speaker_name.Length + 1;
        //	}
        //	fx_text.text = text;
        //	set_color.chars [0] = offset;
        //	set_color.chars [1] += offset;
        //	// Set box height
        //	setBoxHeight ();
        //} 
        throw new System.NotImplementedException();
    }

    public override void setEnabled(bool e)
    {
        gameObject.SetActive(e);
    }
}
