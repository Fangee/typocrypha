using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;

namespace Gameflow
{
    [Node(true, "Dialog/DialogBase", new System.Type[] { typeof(GameflowCanvas) })]
    public abstract class DialogNode : BaseNodeIO
    {
        public override bool AllowRecursion { get { return true; } }

        public string characterName;
        public string text;
    }
}
