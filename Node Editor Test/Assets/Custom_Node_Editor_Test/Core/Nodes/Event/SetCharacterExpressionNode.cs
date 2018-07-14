using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeEditorFramework;

namespace TypocryphaGameflow
{
    [Node(false, "Event/Set Expression", new System.Type[] { typeof(GameflowCanvas) })]
    public class SetCharacterExpressionNode : GameflowStandardIONode
    {
        public const string ID = "Set Character Expression Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Set Expression"; } }
        public override Vector2 MinSize { get { return new Vector2(150, 40); } }

        public string characterName;
        public string expression;

        protected override void OnCreate()
        {
            characterName = "Character Name";
            expression = "default";
        }

        public override void NodeGUI()
        {
            GUILayout.Space(5);
            GUILayout.BeginVertical("box");
            characterName = GUILayout.TextField(characterName);
            expression = GUILayout.TextField(expression);
            GUILayout.EndVertical();
        }
    }
}
