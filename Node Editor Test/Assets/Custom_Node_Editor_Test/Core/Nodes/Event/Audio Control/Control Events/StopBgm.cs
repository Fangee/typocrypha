using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TypocryphaGameflow
{
    public class StopBgm : AudioControlNode.EventData
    {
        public float fadeOut;
        public override void doGUI(Rect rect, int index, IList list)
        {
            Rect UIrect = new Rect(rect);
            UIrect.height = EditorGUIUtility.singleLineHeight;
            GUI.Label(UIrect, new GUIContent("Stop Bgm", ""), new GUIStyle(GUIStyle.none) { alignment = TextAnchor.MiddleCenter });
            UIrect.y += EditorGUIUtility.singleLineHeight + 1;
            GUI.Label(new Rect(UIrect.position, new Vector2(60, EditorGUIUtility.singleLineHeight)), new GUIContent("Fade Time"), GUIStyle.none);
            fadeOut = EditorGUI.FloatField(new Rect(UIrect.position + new Vector2(60, 0), new Vector2(UIrect.width - 65, EditorGUIUtility.singleLineHeight)), fadeOut);
        }
        public override float getHeight(int index)
        {
            return EditorGUIUtility.singleLineHeight * 2 + 2;
        }
    }
}
