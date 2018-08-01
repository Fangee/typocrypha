using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeEditorFramework;

namespace TypocryphaGameflow
{
    [Node(false, "Gameflow/Start", new System.Type[] { typeof(GameflowCanvas) })]
    public class GameflowStartNode : GameflowStandardOutputNode
    {
        public const string ID = "Gameflow Start Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Gameflow Start"; } }
        public override Vector2 MinSize { get { return new Vector2(150, 60); } }

        public override void NodeGUI()
        {
            GUILayout.Space(3);
        }
    }
}
