using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TypocryphaGameflow
{
    public class DebugLog : BattleEvent.Function
    {
        public override float Height { get { return lineHeight * 2; } }
        private static GUIContent titleLabel = new GUIContent("Debug Log", "Prints a string to the Unity console. Allows macro parsing w/ {}");

        public string message;

        public override void doGUI(Rect rect)
        {
            Rect UIRect = new Rect(rect) { height = EditorGUIUtility.singleLineHeight };
            GUI.Label(UIRect, titleLabel);
            UIRect.y += lineHeight;
            GUI.Label(new Rect(UIRect) { width = 60 }, new GUIContent("Message", "The string to print. Allows macro parsing w/ {}"));
            message = GUI.TextField(new Rect(UIRect) { x = UIRect.x + 60, width = UIRect.width - 60 }, message);
        }

        public override bool call(Battlefield field, BattleDataTracker battleData)
        {
            Debug.Log(message);
            return false;
        }
    }
}

