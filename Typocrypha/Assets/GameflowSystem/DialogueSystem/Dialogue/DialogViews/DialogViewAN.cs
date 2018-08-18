using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogViewAN : DialogView
{
    public GameObject dialogBoxPrefab; // Prefab for dialogue box
    public GameObject screenFrame; // Screen frame object
    public RectTransform ANContent; // Content of scroll view
    public GameObject spacebar_icon_an; // Spacebar icon AN view
    public Animator animator_spacebar_an; // Spacebar icon key animator

    public override DialogBox newDialog(DialogItem data)
    {
        #region Check Arguments
        DialogItemAN item = data as DialogItemAN;
        if (item == null)
            throw new System.Exception("Incorrect Type of dialog Item for the AN view mode (requires DialogItemAN)");
        #endregion

        #region Instantiate and initialize new Dialog box
        GameObject obj = GameObject.Instantiate(dialogBoxPrefab, ANContent);
        DialogBox dialogBox = obj.GetComponent<DialogBox>();
        #endregion

        dialogBox.dialogueBoxStart(item);
        return dialogBox;
    }

    public override void setEnabled(bool e)
    {
        gameObject.SetActive(e);
        screenFrame.SetActive(!e);
    }

    // Clear all AN dialogue
    public void clearLog()
    {
        
    }
}
