using UnityEngine;
using UnityEditor;
using NodeEditorFramework;

namespace TypocryphaGameflow
{

    [Node(false, "Event/Set Variable", new System.Type[] { typeof(GameflowCanvas) })]
    public class setVariableNode : GameflowStandardIONode
    {
        public override string Title { get { return "Set Variable"; } }
        public override Vector2 MinSize { get { return new Vector2(150, 50); } }
        public override bool AutoLayout { get { return true; } }

        private const string Id = "Set Variable Node";
        public override string GetID { get { return Id; } }

        public string variableName;
        public string value;

        const string tooltip_varName = "The name of the variable to set. Will not parse macros";
        const string tooltip_setTo = "The name of the variable to set. Will parse macro variables w/ {varName}";

        public override void NodeGUI()
        {
            GUILayout.Space(5);
            GUILayout.BeginVertical("box");
            GUILayout.Label(new GUIContent("Variable Name", tooltip_varName), NodeEditorGUI.nodeLabelBoldCentered);
            variableName = GUILayout.TextField(variableName);
            GUILayout.Label(new GUIContent("Set To", tooltip_setTo), NodeEditorGUI.nodeLabelBoldCentered);
            value = GUILayout.TextField(value);
            GUILayout.EndVertical();
        }

        protected override void OnCreate()
        {
            variableName = "variable-name";
            value = "value";
        }
    }
}
