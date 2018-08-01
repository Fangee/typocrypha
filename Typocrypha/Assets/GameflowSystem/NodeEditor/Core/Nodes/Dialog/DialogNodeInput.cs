using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;

namespace TypocryphaGameflow
{
    [Node(false, "Dialog/Dialog Input", new System.Type[] { typeof(GameflowCanvas) })]
    public class DialogNodeInput : DialogNodeVN
    {
        new public const string ID = "Dialog Input Node";
        public override string Title { get { return "Dialog Input"; } }

        public string variableName;

        protected const string tooltip_saveTo = "Name of the variable to save the input to. Can be set as \"none\" to not save input";

        protected override void OnCreate()
        {
            base.OnCreate();
            variableName = "variable-name";
        }

        public override void NodeGUI()
        {
            base.NodeGUI();
            GUILayout.BeginVertical("box");
            GUILayout.Label(new GUIContent("Save Input To Variable", tooltip_saveTo), NodeEditorGUI.nodeLabelBoldCentered);
            variableName = GUILayout.TextField(variableName);
            GUILayout.Space(5);
            GUILayout.EndVertical();
        }
    }
}
