using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GUIUtilities;

[System.Serializable]
public class SpellAnimationData : ReorderableList<SpellAnimationData>.ListItem {

    public AnimationClip animation;
    public AudioClip sfx;

    public override float Height { get { return lineHeight; } }
    public override void doGUI(Rect rect)
    {
        const int labelWidth = 35;
        const int objectFieldWidth = labelWidth + 53;
        Rect UIRect = new Rect(rect) { height = EditorGUIUtility.singleLineHeight, width = 50 };
        GUI.Label(UIRect, new GUIContent("Anim"));
        UIRect.x += labelWidth + 1;
        UIRect.width = objectFieldWidth;
        animation = EditorGUI.ObjectField(UIRect, animation, typeof(AnimationClip), false) as AnimationClip;
        UIRect.x += objectFieldWidth + 1;
        UIRect.width = labelWidth;
        GUI.Label(UIRect, new GUIContent("Sfx"));
        UIRect.x += labelWidth + 1;
        UIRect.width = objectFieldWidth;
        sfx = EditorGUI.ObjectField(UIRect, sfx, typeof(AudioClip), false) as AudioClip;
    }

}
