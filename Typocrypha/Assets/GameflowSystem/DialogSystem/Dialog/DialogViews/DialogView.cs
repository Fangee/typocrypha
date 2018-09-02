using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DialogView : MonoBehaviour {
    public abstract void setEnabled(bool e);
    public abstract DialogBox newDialog(DialogItem data);
}
