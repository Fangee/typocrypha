using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogViewAN : DialogView
{
    public GameObject dialogBoxPrefab;
    public RectTransform ANContent; // Content of scroll view
    public GameObject spacebar_icon_an; // Spacebar icon AN view
    public Animator animator_spacebar_an; // Spacebar icon key animator

    public override DialogBox newDialog(DialogItem data)
    {
        throw new System.NotImplementedException();
    }

    public override void setEnabled(bool e)
    {
        gameObject.SetActive(e);
    }
}
