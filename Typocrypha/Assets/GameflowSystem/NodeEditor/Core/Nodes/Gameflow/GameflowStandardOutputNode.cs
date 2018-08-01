using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;

namespace TypocryphaGameflow
{

    [Node(true, "Gameflow/Standard Output Base", new System.Type[] { typeof(GameflowCanvas) })]
    public abstract class GameflowStandardOutputNode : GameflowNode
    {
        //Next Node to go to (OUTPUT)
        [ConnectionKnob("To Next", Direction.Out, "Gameflow", ConnectionCount.Single, NodeSide.Right, 30)]
        public ConnectionKnob toNextOUT;
        public override BaseGameflowNode next()
        {
            return toNextOUT.connections[0].body as BaseGameflowNode;
        }
    }
}
