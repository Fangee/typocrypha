using UnityEngine;
using UnityEditor;
using NodeEditorFramework;

namespace TypocryphaGameflow
{
    [Node(false, "Dialog/VN Dialog Node", new System.Type[]{typeof(GameflowCanvas) })]
    public class DialogueNodeVN : BaseDialogNode
    {
        public const string ID = "VN Dialog Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "VN Dialog Node"; } }
        public override Vector2 MinSize { get { return new Vector2(300, 60); } }
        public override bool AutoLayout { get { return true; } }

        //Connection from previous node (INPUT)
        [ConnectionKnob("From Previous", Direction.In, "Gameflow", NodeSide.Left, 30)]
        public ConnectionKnob fromPreviousIN;
        //Next Node to go to (OUTPUT)
        [ConnectionKnob("To Next", Direction.Out, "Gameflow", NodeSide.Right, 30)]
        public ConnectionKnob toNextOUT;

        public string expression;

        private Vector2 scroll;
        protected static GUIStyle labelStyle = new GUIStyle();

        private const string tooltip_name = "The speaking character's name. Used to set speaking sfx and sprite highlighting if not overriden by text events";
        private const string tooltip_expr = "The speaking character's expression. Only needs to be set if the expression should be changed";

        protected override void OnCreate()
        {
            characterName = "Character Name";
            expression = "Expression";
            dialogText = "Insert dialog text here";
        }

        public override void NodeGUI()
        {
            labelStyle.normal.textColor = Color.white;
            GUILayout.Space(5);
            EditorGUILayout.BeginVertical("Box");
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Name", tooltip_name) , labelStyle, GUILayout.Width(45f));
            characterName = EditorGUILayout.TextField("", characterName, GUILayout.Width(235f));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Expr", tooltip_expr), labelStyle, GUILayout.Width(45f));
            expression = EditorGUILayout.TextField("", expression, GUILayout.Width(235f));
            GUILayout.EndHorizontal();
            GUILayout.Space(3);
            GUILayout.EndVertical();

            GUILayout.Label(new GUIContent("Dialogue Text", "add tooltip here"), NodeEditorGUI.nodeLabelBoldCentered);

            GUILayout.BeginHorizontal();

            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(100));
            EditorStyles.textField.wordWrap = true;
            dialogText = EditorGUILayout.TextArea(dialogText, GUILayout.ExpandHeight(true));
            EditorStyles.textField.wordWrap = false;
            EditorGUILayout.EndScrollView();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 90;
            GUILayout.EndHorizontal();
        }
    }

}
