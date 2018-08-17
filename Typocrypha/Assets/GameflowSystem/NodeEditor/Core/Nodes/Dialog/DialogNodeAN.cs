using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeEditorFramework;

namespace TypocryphaGameflow
{
    [Node(false, "Dialog/AN Dialog", new System.Type[] { typeof(GameflowCanvas) })]
    public class DialogNodeAN : DialogNode
    {
        #region Editor
        public const string ID = "AN Dialog Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "AN Dialog"; } }
        public override Vector2 MinSize { get { return new Vector2(250, 60); } }

        protected static GUIStyle labelStyle = new GUIStyle();

        #region Tooltip Strings
        private const string tooltip_name = "The speaking character's name (optional). Used to set speaking sfx if not overriden by text events";
        protected const string tooltip_text = "The text to be displayed. Can substitute text macros using {macro-name,args}, and call text events using [event-name,args]";
        #endregion

        protected override void OnCreate()
        {
            characterName = "Name (optional)";
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
            GUILayout.Space(3);
            GUILayout.EndVertical();

            GUILayout.Label(new GUIContent("Dialog Text", tooltip_text), NodeEditorGUI.nodeLabelBoldCentered);

            GUILayout.BeginHorizontal();
            GUIStyle dialogTextStyle = new GUIStyle(EditorStyles.textArea);
            dialogTextStyle.wordWrap = true;
            text = EditorGUILayout.TextArea(text, dialogTextStyle, GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 5));
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
            Debug.Log("AN: " + text);
            managers.dialogManager.setEnabled(true);
            managers.battleManager.setEnabled(false);
            //TODO: Set expression if necessary
            managers.characterManager.speak(characterName);
            managers.dialogManager.startDialogLine(DialogManager.DialogViewType.AN, new DialogItemAN(text));
            return ProcessFlag.Wait;
        }
        #endregion
    }

}