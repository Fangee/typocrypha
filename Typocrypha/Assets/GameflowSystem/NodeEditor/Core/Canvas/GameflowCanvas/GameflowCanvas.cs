using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;

namespace Gameflow
{

    [NodeCanvasType("Gameflow Canvas")]
    public class GameflowCanvas : NodeCanvas
    {
        public override string canvasName { get { return "Gameflow"; } }

        public GameflowStartNode getStartNode()
        {
            return nodes.Find((node) => { return node is GameflowStartNode; }) as GameflowStartNode;
        }

    }
}
