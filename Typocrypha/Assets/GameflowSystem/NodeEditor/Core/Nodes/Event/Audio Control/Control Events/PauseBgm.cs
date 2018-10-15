using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Gameflow
{
    public class PauseBgm : AudioControlNode.EventData
    {
        public override void doGUI(Rect rect)
        {
            Rect UIrect = new Rect(rect);
            UIrect.height = EditorGUIUtility.singleLineHeight;
            GUI.Label(UIrect, new GUIContent("Pause Bgm", ""), new GUIStyle(GUIStyle.none) { alignment = TextAnchor.MiddleCenter });
        }
        public override float Height
        {
            get
            {
                return EditorGUIUtility.singleLineHeight * 1 + 1;
            }
        }
    }
}