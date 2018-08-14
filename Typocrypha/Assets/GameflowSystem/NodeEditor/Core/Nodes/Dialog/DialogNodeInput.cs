
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using NodeEditorFramework;

namespace TypocryphaGameflow
{
    [Node(false, "Dialog/Dialog Input", new System.Type[] { typeof(GameflowCanvas) })]
    public class DialogNodeInput : DialogNodeVN
    {
        new public const string ID = "Dialog Input Node";
        public override string Title { get { return "Dialog Input"; } }

        public string variableName;
        public bool showChoicePrompt;
        [SerializeField]
        AnimBool showChoiceAnimBool;
        public string[] choicePromptText;

        protected const string tooltip_saveTo = "Name of the variable to save the input to. Can be set as \"none\" to not save input";
        protected const string tooltip_showPrompt = "Show input prompts (AB w/ optional C). The choice will still branch arbitrarily";

        protected override void OnCreate()
        {
            base.OnCreate();
            characterName = "PROMPT";
            expression = string.Empty;
            variableName = "variable-name";
            showChoicePrompt = false;
            showChoiceAnimBool = new AnimBool(showChoicePrompt);
            showChoiceAnimBool.speed = 5;
            choicePromptText = new string[3];
        }

        public override void NodeGUI()
        {
            base.NodeGUI();
            GUILayout.BeginVertical("box");
            GUILayout.Label(new GUIContent("Save Input To Variable", tooltip_saveTo), NodeEditorGUI.nodeLabelBoldCentered);
            variableName = GUILayout.TextField(variableName);
            GUILayout.Space(5);
            GUILayout.EndVertical();
            GUILayout.BeginVertical("box");
            showChoiceAnimBool.target = EditorGUILayout.ToggleLeft("Show Choice Prompts?", showChoiceAnimBool.target, NodeEditorGUI.nodeLabelBoldCentered);
            showChoicePrompt = showChoiceAnimBool.target;
            //Extra block that can be toggled on and off.
            using (var group = new EditorGUILayout.FadeGroupScope(showChoiceAnimBool.faded))
            {
                if (group.visible)
                {
                    EditorGUI.indentLevel++;
                    choicePromptText[0] = GUILayout.TextField(choicePromptText[0]);
                    choicePromptText[1] = GUILayout.TextField(choicePromptText[1]);
                    choicePromptText[2] = GUILayout.TextField(choicePromptText[2]);
                    EditorGUI.indentLevel--;
                }
            }
            GUILayout.EndVertical();
        }
    }
}
