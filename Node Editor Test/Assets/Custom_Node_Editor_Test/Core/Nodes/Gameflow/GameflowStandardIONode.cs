using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;

namespace TypocryphaGameflow
{

    [Node(true, "Gameflow/Standard IO Base", new System.Type[] { typeof(GameflowCanvas) })]
    public abstract class GameflowStandardIONode : Node
    {

        public override bool AutoLayout { get { return true; } }
        //Connection from previous node (INPUT)
        [ConnectionKnob("From Previous", Direction.In, "Gameflow", ConnectionCount.Multi, NodeSide.Left, 30)]
        public ConnectionKnob fromPreviousIN;
        //Next Node to go to (OUTPUT)
        [ConnectionKnob("To Next", Direction.Out, "Gameflow", ConnectionCount.Single, NodeSide.Right, 30)]
        public ConnectionKnob toNextOUT;
    }
}
