using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeEditorFramework;

namespace TypocryphaGameflow
{
    [Node(false, "Gameflow/Start", new System.Type[] { typeof(GameflowCanvas) })]
    public class GameflowStartNode : Node
    {
        public const string ID = "Gameflow Start Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Gameflow Start"; } }
        public override Vector2 MinSize { get { return new Vector2(300, 60); } }
        public override bool AutoLayout { get { return true; } }

        protected static GUIStyle labelStyle = new GUIStyle();

        public string gameflowID;
        public const string tooltip_gameflowID = "The ID for this branch of this gameflow canvas. Can be used as a starting point for the canvas or as a point to jump to.";

        //Next Node to go to (OUTPUT)
        [ConnectionKnob("To Next", Direction.Out, "Gameflow", NodeSide.Right, 30)]
        public ConnectionKnob toNextOUT;

        protected override void OnCreate()
        {
            gameflowID = "Gameflow ID";
        }

        public override void NodeGUI()
        {
            labelStyle.normal.textColor = Color.white;
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Name", tooltip_gameflowID), labelStyle, GUILayout.Width(45f));
            gameflowID = EditorGUILayout.TextField("", gameflowID, GUILayout.Width(235f));
            EditorGUILayout.EndHorizontal();
        }
    }
}
