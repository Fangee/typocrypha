using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;

namespace TypocryphaGameflow
{
    [Node(false, "Dialog/Dialog Input", new System.Type[] { typeof(GameflowCanvas) })]
    public class DialogNodeInput : DialogNodeVN
    {
        new public const string ID = "inputDialogNode";
        public override string Title { get { return "Input Dialog Node"; } }
    }
}
