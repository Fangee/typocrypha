using UnityEngine;
using UnityEditor;
using NodeEditorFramework;

namespace TypocryphaGameflow
{
    [Node(false, "Dialog/VN Dialog", new System.Type[] { typeof(GameflowCanvas) })]
    public class DialogNodeVN : DialogNode
    {
        #region Editor
        public const string ID = "VN Dialog Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "VN Dialog"; } }
        public override Vector2 MinSize { get { return new Vector2(250, 60); } }

        public string expression;

        protected static GUIStyle labelStyle = new GUIStyle();

        #region Tooltip Strings
        private const string tooltip_name = "The speaking character's name. Used to set speaking sfx and sprite highlighting if not overriden by text events";
        private const string tooltip_expr = "The speaking character's expression. Only needs to be set if the expression should be changed";
        protected const string tooltip_text = "The text to be displayed. Can substitute text macros using {macro-name,args}, and call text events using [event-name,args]";
        #endregion

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
            characterName = EditorGUILayout.TextField("", characterName, GUILayout.Width(MinSize.x - 65));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Expr", tooltip_expr), labelStyle, GUILayout.Width(45f));
            expression = EditorGUILayout.TextField("", expression, GUILayout.Width(MinSize.x - 65));
            GUILayout.EndHorizontal();
            GUILayout.Space(3);
            GUILayout.EndVertical();

            GUILayout.Label(new GUIContent("Dialog Text", tooltip_text), NodeEditorGUI.nodeLabelBoldCentered);

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
        #endregion

        #region Game
        public override ProcessFlag process(GameManagers managers)
        {
            Debug.Log("VN: " + characterName + ": " + text);
            managers.dialogManager.setEnabled(true);
            managers.battleManager.setEnabled(false);
            //TODO: Set expression if necessary
            managers.characterManager.speak(characterName);
            managers.dialogManager.startDialogLine(DialogManager.DialogViewType.VN, new DialogItemVN(text, characterName, null, null));
            return ProcessFlag.Wait;
        }
        #endregion
    }
}
