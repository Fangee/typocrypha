using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;

namespace TypocryphaGameflow
{
    [Node(true, "Gameflow/GameflowBase", new System.Type[] { typeof(GameflowCanvas) })]
    public abstract class GameflowNode : BaseGameflowNode
    {
        public override bool AutoLayout { get { return true; } }
    }
}
