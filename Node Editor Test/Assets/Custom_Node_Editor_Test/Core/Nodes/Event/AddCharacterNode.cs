using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;

namespace TypocryphaGameflow
{
    [Node(false, "Event/Add Character", new System.Type[] { typeof(GameflowCanvas) })]
    public class AddCharacterNode : GameflowStandardIONode
    {
        public const string ID = "Add Character Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Add Character"; } }
        public override Vector2 MinSize { get { return new Vector2(150, 40); } }

        public string characterName;

        protected override void OnCreate()
        {
            characterName = "Character Name";
        }

        public override void NodeGUI()
        {
            characterName = GUILayout.TextField(characterName);
        }
    }
}
