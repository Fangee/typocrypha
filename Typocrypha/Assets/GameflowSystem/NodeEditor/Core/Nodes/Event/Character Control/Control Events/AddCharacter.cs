using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TypocryphaGameflow
{
    public class AddCharacter : CharacterControlNode.EventData
    {
        public string startingPose = "base pose";
        public string startingExpression = "base expression";
        public Vector2 pos = new Vector2(0, 0);

        public override void characterControl(Character character)
        {
            // Intercepted in CharacterManager
        }

        #region GUI
        public override void doGUI(Rect rect)
        {
            Rect UIrect = new Rect(rect);
            UIrect.height = EditorGUIUtility.singleLineHeight;
            GUI.Label(UIrect, new GUIContent("Add Character", ""), new GUIStyle(GUIStyle.none) { alignment = TextAnchor.MiddleCenter });
            UIrect.y += EditorGUIUtility.singleLineHeight + 1;
            characterName = GUI.TextField(UIrect, characterName);
            UIrect.y += EditorGUIUtility.singleLineHeight + 1;
            startingPose = GUI.TextField(UIrect, startingPose);
            UIrect.y += EditorGUIUtility.singleLineHeight + 1;
            startingExpression = GUI.TextField(UIrect, startingExpression);
            UIrect.y += EditorGUIUtility.singleLineHeight + 1;
            pos = EditorGUI.Vector2Field(UIrect, "", pos);
        }
        public override float Height
        {
            get
            {
                return EditorGUIUtility.singleLineHeight * 5 + 5;
            }
        }
        #endregion
    }
}