using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;

[Node(false, "Event/Add Character", new System.Type[] { typeof(GameflowCanvas) })]
public class AddCharacterNode : GameflowStandardIONode {
    public const string ID = "Add Character Node";
    public override string GetID { get { return ID; } }
    public string characterName;
    public override Vector2 MinSize { get { return new Vector2(150, 40); } }
    protected override void OnCreate()
    {
        characterName = "Character Name";
    }

    public override void NodeGUI()
    {
        characterName = GUILayout.TextField(characterName);
    }
}
