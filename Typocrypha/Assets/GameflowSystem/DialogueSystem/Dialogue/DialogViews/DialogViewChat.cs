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
        #region Check Arguments
        DialogItemChat item = data as DialogItemChat;
        if (item == null)
            throw new System.Exception("Incorrect Type of dialog Item for the Chat view mode (requires DialogItemChat)");
        #endregion

        #region Instantiate and initialize new Dialog box
        GameObject obj = GameObject.Instantiate(dialogBoxPrefab);
        SpriteRenderer leftIcon = obj.transform.GetChild(1).GetComponent<SpriteRenderer>();
        SpriteRenderer rightIcon = obj.transform.GetChild(2).GetComponent<SpriteRenderer>();
        if (item.iconSide == IconSide.LEFT || item.iconSide == IconSide.BOTH)
        {
            leftIcon.sprite = item.leftIcon;
            leftIcon.enabled = true;
        }
        if (item.iconSide == IconSide.RIGHT || item.iconSide == IconSide.BOTH)
        {
            rightIcon.sprite = item.rightIcon;
            rightIcon.enabled = true;
        }
        DialogBox dialogBox = obj.GetComponent<DialogBox>();
        #endregion

        //MAY BE RELEVANT (USED TO BE IN THE DIALOG BOX)
        //	// Add text with speaker's name, and offset text display (ADD TO ITEM's TEXT)
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

        //TODO ACTUAL INTEGRATION WITH CHAT WINDOW STUFF (SEE DEPRECATED DIALOGUE MANAGER - CHAT WINDOW CONTROLS region)
        throw new System.NotImplementedException("Chat window integration not finished");

        dialogBox.dialogueBoxStart(item);
        return dialogBox;
    }

    public override void setEnabled(bool e)
    {
        gameObject.SetActive(e);
    }
}
