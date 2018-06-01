using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogueItemVN))]
//[CanEditMultipleObjects]
public class DialogueItemVNEditor : Editor
{
    SerializedProperty dialogueType;
    SerializedProperty speakerName;
    SerializedProperty text;
    SerializedProperty inputDisplay;
    SerializedProperty inputOptions;
    SerializedProperty inputAnswers;
    SerializedProperty inputBranches;
    bool spriteOptions = false;
    SerializedProperty mcSprite; // Sprite of main character (displayed in text box)
    SerializedProperty codecSprite; // Sprite of main character (displayed in text box)
    SerializedProperty charSprites; // Character sprites displayed on screen (VN MODE)
    SerializedProperty charSpritePos; // Positions of character sprites (VN MODE)

    void OnEnable()
    {
        dialogueType = serializedObject.FindProperty("dialogue_type");
        speakerName = serializedObject.FindProperty("speaker_name");
        text = serializedObject.FindProperty("text");
        inputDisplay = serializedObject.FindProperty("input_display");
        inputOptions = serializedObject.FindProperty("input_options");
        inputAnswers = serializedObject.FindProperty("input_answers");
        inputBranches = serializedObject.FindProperty("input_branches");
        mcSprite = serializedObject.FindProperty("mc_sprite");
        codecSprite = serializedObject.FindProperty("codec_sprite");
        charSprites = serializedObject.FindProperty("char_sprites");
        charSpritePos = serializedObject.FindProperty("char_sprite_pos");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(dialogueType, new GUIContent("Mode"));
        EditorGUILayout.PropertyField(speakerName, new GUIContent("Speaker"));
        EditorGUILayout.PropertyField(text);
        if (dialogueType.enumValueIndex == (int)DialogueType.INPUT)
        {
            EditorGUILayout.PropertyField(inputDisplay);
            EditorGUILayout.PropertyField(inputOptions, true);
            EditorGUILayout.PropertyField(inputAnswers, true);
            EditorGUILayout.PropertyField(inputBranches, true);
        }
        spriteOptions = EditorGUILayout.Foldout(spriteOptions, new GUIContent("Sprite Options"));
        if (spriteOptions)
        {
            EditorGUILayout.PropertyField(mcSprite);
            EditorGUILayout.PropertyField(codecSprite);
            EditorGUILayout.PropertyField(charSprites, true);
            EditorGUILayout.PropertyField(charSpritePos, true);
        }
        //inputBranches.stringValue = EditorGUILayout.TextArea(inputBranches.stringValue, GUILayout.MinHeight(128));
        serializedObject.ApplyModifiedProperties();
    }
}

