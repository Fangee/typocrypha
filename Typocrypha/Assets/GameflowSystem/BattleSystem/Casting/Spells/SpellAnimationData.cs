using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GUIUtilities;

[System.Serializable]
public class SpellAnimationData : ReorderableList<SpellAnimationData>.ListItem {
    private bool expand = false;
    public List<AnimationClip> animations;
    public List<AudioClip> sfx;

    public override float Height { get { return lineHeight * (1 + (expand ? System.Math.Max(animations.Count, sfx.Count) : 0)); } }
    public override void doGUI(Rect rect)
    {
        const int labelWidth = 35;
        const int objectFieldWidth = labelWidth + 53;
        Rect UIRect = new Rect(rect) { height = EditorGUIUtility.singleLineHeight, width = 82};
        GUI.Label(UIRect, new GUIContent("Data (Parallel)", "TODO: tooltip"));
        UIRect.width = 50;
        UIRect.x += 133;
        GUI.Button(UIRect, new GUIContent("+ Anim", "TODO: tooltip"));
        UIRect.width = 50;
        UIRect.x += 51;
        GUI.Button(UIRect, new GUIContent("+ Sfx", "TODO: tooltip"));
        //UIRect.x += labelWidth + 1;
        //UIRect.width = objectFieldWidth;
        //animation = EditorGUI.ObjectField(UIRect, animation, typeof(AnimationClip), false) as AnimationClip;
        //UIRect.x += objectFieldWidth + 1;
        //UIRect.width = labelWidth;
        //GUI.Label(UIRect, new GUIContent("Sfx"));
        //UIRect.x += labelWidth + 1;
        //UIRect.width = objectFieldWidth;
        //sfx = EditorGUI.ObjectField(UIRect, sfx, typeof(AudioClip), false) as AudioClip;
    }

}
