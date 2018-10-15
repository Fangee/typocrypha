using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Gameflow
{
    public class RemoveCharacter : CharacterControlNode.EventData
    {
        public override void characterControl(Character character)
        {
            // Intercepted in CharacterManager
        }

        #region GUI
        public override void doGUI(Rect rect)
        {
            Rect UIrect = new Rect(rect);
            UIrect.height = EditorGUIUtility.singleLineHeight;
            GUI.Label(UIrect, new GUIContent("Remove Character", ""), new GUIStyle(GUIStyle.none) { alignment = TextAnchor.MiddleCenter });
            UIrect.y += EditorGUIUtility.singleLineHeight;
            characterName = GUI.TextField(UIrect, characterName);
        }
        #endregion
    }
}
