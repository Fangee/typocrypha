﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TypocryphaGameflow
{
    public class UnpauseBgm : AudioControlNode.EventData
    {
        public override void doGUI(Rect rect)
        {
            Rect UIrect = new Rect(rect);
            UIrect.height = EditorGUIUtility.singleLineHeight;
            GUI.Label(UIrect, new GUIContent("Unpause Bgm", ""), new GUIStyle(GUIStyle.none) { alignment = TextAnchor.MiddleCenter });
        }
        public override float getHeight()
        {
            return EditorGUIUtility.singleLineHeight * 1 + 1;
        }
    }
}