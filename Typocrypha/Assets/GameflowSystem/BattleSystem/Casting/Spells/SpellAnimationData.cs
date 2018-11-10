using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GUIUtilities;

[System.Serializable]
public class SpellAnimationData : ReorderableList<SpellAnimationData>.ListItem {
    private bool expand = true;
    public List<AnimationClip> animations = new List<AnimationClip>();
    public List<AudioClip> sfx = new List<AudioClip>();

    public override float Height
    {
        get
        {
            int mult = 1;
            if(expand)
            {
                if (sfx.Count > 0)
                    mult += sfx.Count + 1;
                if (animations.Count > 0)
                    mult += animations.Count + 1;
            }
            return lineHeight * mult + 1;
        }
    }
    public override void doGUI(Rect rect)
    {
        #region Object Picker Message Handling
        Event e = Event.current;
        if (e.type == EventType.ExecuteCommand && e.commandName == "ObjectSelectorClosed")
        {
            e.Use();
            AnimationClip anim = EditorGUIUtility.GetObjectPickerObject() as AnimationClip;
            if (anim != null)
            {
                animations.Add(anim);
                return;
            }
            AudioClip sfxClip = EditorGUIUtility.GetObjectPickerObject() as AudioClip;
            if (sfxClip != null)
            {
                sfx.Add(sfxClip);
            }
        }
        #endregion

        const int labelWidth = 35;
        const int objectFieldWidth = labelWidth + 53;
        Rect UIRect = new Rect(rect) { height = EditorGUIUtility.singleLineHeight, width = 82};
        GUI.Label(UIRect, new GUIContent("Data (Parallel)", "TODO: tooltip"));
        UIRect.width = 50;
        UIRect.x += 153;
        if(GUI.Button(UIRect, new GUIContent("+ Anim", "TODO: tooltip")))
            EditorGUIUtility.ShowObjectPicker<AnimationClip>(null, false, "", 1);
        UIRect.width = 50;
        UIRect.x += 51;
        if(GUI.Button(UIRect, new GUIContent("+ Sfx", "TODO: tooltip")))
            EditorGUIUtility.ShowObjectPicker<AudioClip>(null, false, "", 1);
        if (expand)
        {
            UIRect = new Rect(rect) { height = EditorGUIUtility.singleLineHeight };
            UIRect.y += lineHeight;
            if (animations.Count > 0)
            {
                GUI.Label(UIRect, new GUIContent("Animations", "TODO:tooltip"));
                UIRect.y += lineHeight;
                for (int i = 0; i < animations.Count; ++i)
                {
                    animations[i] = EditorGUI.ObjectField(UIRect, animations[i], typeof(AnimationClip), false) as AnimationClip;
                    UIRect.y += lineHeight;
                }
            }
            if (sfx.Count > 0)
            {
                GUI.Label(UIRect, new GUIContent("Sfx", "TODO:tooltip"));
                UIRect.y += lineHeight;
                for (int i = 0; i < sfx.Count; ++i)
                {
                    sfx[i] = EditorGUI.ObjectField(UIRect, sfx[i], typeof(AudioClip), false) as AudioClip;
                    UIRect.y += lineHeight;
                }
            }
        }
    }

}