using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;

namespace Gameflow
{

    [Node(true, "Gameflow/Standard IO Base", new System.Type[] { typeof(GameflowCanvas) })]
    public abstract class BaseNodeIO : BaseNodeOUT
    {
        //Connection from previous node (INPUT)
        [ConnectionKnob("From Previous", Direction.In, "Gameflow", ConnectionCount.Multi, NodeSide.Left, 30)]
        public ConnectionKnob fromPreviousIN;
    }
}
