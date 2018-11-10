using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AnimationTest))]
public class AnimationTestEditor : Editor
{
    GUIUtilities.ReorderableList<SpellAnimationData> data = null;
    AnimationTest curr = null;
    public override void OnInspectorGUI()
    {
        AnimationTest myTarget = (AnimationTest)target;
        myTarget.CompletionTriggers = EditorGUILayout.Toggle(new GUIContent("use completion triggers"), myTarget.CompletionTriggers);
        myTarget.speed = EditorGUILayout.FloatField(new GUIContent("speed"), myTarget.speed);
        if (data == null || myTarget != curr)
        {
            data = new GUIUtilities.ReorderableList<SpellAnimationData>(myTarget.clips, true, true, new GUIContent("Animation Data"));
            curr = myTarget;
        }
        data.doLayoutList();
    }
}
