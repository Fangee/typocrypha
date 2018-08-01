using UnityEngine;
using UnityEditor;
using NodeEditorFramework;

namespace TypocryphaGameflow
{
    [Node(false, "Dialog/VN Dialog", new System.Type[] { typeof(GameflowCanvas) })]
    public class DialogNodeVN : DialogManagerNode
    {
        public const string ID = "VN Dialog Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "VN Dialog"; } }
        public override Vector2 MinSize { get { return new Vector2(300, 60); } }
        public override bool AutoLayout { get { return true; } }

        public string expression;

        protected static GUIStyle labelStyle = new GUIStyle();

        private const string tooltip_name = "The speaking character's name. Used to set speaking sfx and sprite highlighting if not overriden by text events";
        private const string tooltip_expr = "The speaking character's expression. Only needs to be set if the expression should be changed";
        protected const string tooltip_text = "The text to be displayed. Can substitute text macros using {macro-name,args}, and call text events using [event-name,args]";

        protected override void OnCreate()
        {
            characterName = "Character Name";
            expression = "Expression";
            text = "Insert dialog text here";
        }

        public override void NodeGUI()
        {
            labelStyle.normal.textColor = Color.white;
            GUILayout.Space(5);
            EditorGUILayout.BeginVertical("Box");
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Name", tooltip_name), labelStyle, GUILayout.Width(45f));
            characterName = EditorGUILayout.TextField("", characterName, GUILayout.Width(235f));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Expr", tooltip_expr), labelStyle, GUILayout.Width(45f));
            expression = EditorGUILayout.TextField("", expression, GUILayout.Width(235f));
            GUILayout.EndHorizontal();
            GUILayout.Space(3);
            GUILayout.EndVertical();

            GUILayout.Label(new GUIContent("Dialogue Text", tooltip_text), NodeEditorGUI.nodeLabelBoldCentered);

            GUILayout.BeginHorizontal();
            GUIStyle dialogTextStyle = new GUIStyle(EditorStyles.textArea);
            dialogTextStyle.wordWrap = true;
            text = EditorGUILayout.TextArea(text, dialogTextStyle,GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 5));
            GUILayout.EndHorizontal();

            //Don't know why this code needs to be here exactly, but it makes everything nicer? maybe add to some static stuff?
            GUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 90;
            GUILayout.EndHorizontal();
        }
    }

}
