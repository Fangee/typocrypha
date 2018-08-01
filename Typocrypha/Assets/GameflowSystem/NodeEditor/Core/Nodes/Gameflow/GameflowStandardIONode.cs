using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;

namespace TypocryphaGameflow
{

    [Node(true, "Gameflow/Standard IO Base", new System.Type[] { typeof(GameflowCanvas) })]
    public abstract class GameflowStandardIONode : GameflowStandardOutputNode
    {
        //Connection from previous node (INPUT)
        [ConnectionKnob("From Previous", Direction.In, "Gameflow", ConnectionCount.Multi, NodeSide.Left, 30)]
        public ConnectionKnob fromPreviousIN;
    }
}
