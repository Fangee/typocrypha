using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TypocryphaGameflow
{
    public class PlayBgm : AudioControlNode.EventData
    {
        public AudioClip bgm;
        public float fadeIn;
        public override void doGUI(Rect rect, int index, IList list)
        {
            Rect UIrect = new Rect(rect);
            UIrect.height = EditorGUIUtility.singleLineHeight;
            GUI.Label(UIrect, new GUIContent("Play Bgm", ""), new GUIStyle(GUIStyle.none) { alignment = TextAnchor.MiddleCenter });
            UIrect.y += EditorGUIUtility.singleLineHeight + 1;
            bgm = EditorGUI.ObjectField(UIrect, bgm, typeof(AudioClip), false) as AudioClip;
            UIrect.y += EditorGUIUtility.singleLineHeight + 1;
            GUI.Label(new Rect(UIrect.position, new Vector2(60, EditorGUIUtility.singleLineHeight)), new GUIContent("Fade Time"), GUIStyle.none);
            fadeIn = EditorGUI.FloatField(new Rect(UIrect.position + new Vector2(60,0), new Vector2(UIrect.width - 65, EditorGUIUtility.singleLineHeight)), fadeIn);
        }
        public override float getHeight(int index)
        {
            return EditorGUIUtility.singleLineHeight * 3 + 2;
        }
    }
}
