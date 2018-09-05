using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TypocryphaGameflow
{
    public class PlaySfx : AudioControlNode.EventData
    {
        public AudioClip bgm;
        public override void doGUI(Rect rect)
        {
            Rect UIrect = new Rect(rect);
            UIrect.height = EditorGUIUtility.singleLineHeight;
            GUI.Label(UIrect, new GUIContent("Play Sfx", ""), new GUIStyle(GUIStyle.none) { alignment = TextAnchor.MiddleCenter });
            UIrect.y += EditorGUIUtility.singleLineHeight + 1;
            bgm = EditorGUI.ObjectField(UIrect, bgm, typeof(AudioClip), false) as AudioClip;
        }
        public override float getHeight()
        {
            return EditorGUIUtility.singleLineHeight * 2 + 2;
        }
    }
}