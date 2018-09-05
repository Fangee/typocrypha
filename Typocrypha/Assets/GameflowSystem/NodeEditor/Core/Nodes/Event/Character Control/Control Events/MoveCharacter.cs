using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TypocryphaGameflow
{
    public class MoveCharacter : CharacterControlNode.EventData
    {
        public enum MovementType
        {
            Set_Position,
            Fade_In,
            Fade_Out,
            Crossfade,
            Slide,
        }
        public MovementType moveType = MovementType.Set_Position;
        public Vector2 targetPos = new Vector2(0, 0);
        public float time = 1.0f;
        private int addLines = 0;

        public override void characterControl(Character character)
        {
            switch(moveType)
            {
                case MovementType.Set_Position:
                    character.teleport(targetPos);
                    break;
                default:
                    break;
            }
            
        }

        #region GUI
        public override void doGUI(Rect rect)
        {
            Rect UIrect = new Rect(rect);
            UIrect.height = EditorGUIUtility.singleLineHeight;
            GUI.Label(UIrect, new GUIContent("Move Character", ""), new GUIStyle(GUIStyle.none) { alignment = TextAnchor.MiddleCenter });
            UIrect.y += EditorGUIUtility.singleLineHeight + 1;
            characterName = GUI.TextField(UIrect, characterName);
            UIrect.y += EditorGUIUtility.singleLineHeight + 1;
            moveType = (MovementType)EditorGUI.EnumPopup(UIrect, moveType);
            UIrect.y += EditorGUIUtility.singleLineHeight + 1;
            if (moveType != MovementType.Set_Position)
            {
                time = EditorGUI.FloatField(UIrect, time);
                UIrect.y += EditorGUIUtility.singleLineHeight + 1;
                addLines = 1;
            }
            else
                addLines = 0;
            targetPos = EditorGUI.Vector2Field(UIrect, "", targetPos);
        }

        public override float getHeight()
        {
            return EditorGUIUtility.singleLineHeight * (4 + addLines) + (4 + addLines);
        }
        #endregion
    }

}
