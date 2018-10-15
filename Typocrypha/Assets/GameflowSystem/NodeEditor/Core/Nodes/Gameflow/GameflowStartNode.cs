using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeEditorFramework;

namespace Gameflow
{
    [Node(false, "Gameflow/Start", new System.Type[] { typeof(GameflowCanvas) })]
    public class GameflowStartNode : BaseNodeOUT
    {
        public const string ID = "Gameflow Start Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Gameflow Start"; } }
        public override Vector2 MinSize { get { return new Vector2(150, 60); } }

        public override void NodeGUI()
        {
            GUILayout.Space(3);
        }

        public override ProcessFlag process(GameManagers managers)
        {
            Debug.Log("Gameflow start");
            return ProcessFlag.Continue;
        }
    }
}
