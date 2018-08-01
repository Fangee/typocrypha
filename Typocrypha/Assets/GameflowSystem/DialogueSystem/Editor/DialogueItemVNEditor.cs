using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[CustomEditor(typeof(DialogueItemVN))]
////[CanEditMultipleObjects]
//public class DialogueItemVNEditor : Editor
//{
//    SerializedProperty dialogueType;
//    SerializedProperty speakerName;
//    SerializedProperty text;
//    SerializedProperty inputDisplay;
//    SerializedProperty inputOptions;
//    SerializedProperty inputAnswers;
//    SerializedProperty inputBranches;
//    bool spriteOptions = false;
//    SerializedProperty mcSprite; // Sprite of main character (displayed in text box)
//    SerializedProperty codecSprite; // Sprite of main character (displayed in text box)
//    SerializedProperty charSprites; // Character sprites displayed on screen (VN MODE)
//    SerializedProperty charSpritePos; // Positions of character sprites (VN MODE)

//    void OnEnable()
//    {
//        dialogueType = serializedObject.FindProperty("dialogue_type");
//        speakerName = serializedObject.FindProperty("speaker_name");
//        text = serializedObject.FindProperty("text");
//        inputDisplay = serializedObject.FindProperty("input_display");
//        inputOptions = serializedObject.FindProperty("input_options");
//        inputAnswers = serializedObject.FindProperty("input_answers");
//        inputBranches = serializedObject.FindProperty("input_branches");
//        mcSprite = serializedObject.FindProperty("mc_sprite");
//        codecSprite = serializedObject.FindProperty("codec_sprite");
//        charSprites = serializedObject.FindProperty("char_sprites");
//        charSpritePos = serializedObject.FindProperty("char_sprite_pos");
//    }

//    public override void OnInspectorGUI()
//    {
//        serializedObject.Update();
//        EditorGUILayout.PropertyField(dialogueType, new GUIContent("Mode"));
//        EditorGUILayout.PropertyField(speakerName, new GUIContent("Speaker Name"));
//        EditorGUILayout.PropertyField(text);
//        spriteOptions = EditorGUILayout.Foldout(spriteOptions, new GUIContent("Sprite Options"));
//        if (spriteOptions)
//        {
//            EditorGUILayout.PropertyField(mcSprite, new GUIContent("Set MC Sprite"));
//            EditorGUILayout.PropertyField(codecSprite, new GUIContent("Set Codec Sprite"));
//            EditorGUILayout.PropertyField(charSprites, new GUIContent("Add Character Sprites"), true);
//            EditorGUILayout.PropertyField(charSpritePos, new GUIContent("Add Character Sprite Positions"), true);
//        }
//        if (dialogueType.enumValueIndex == (int)DialogueType.INPUT)
//        {
//            EditorGUILayout.PropertyField(inputDisplay, new GUIContent("Input Display"));
//            EditorGUILayout.PropertyField(inputOptions, new GUIContent("Input Options"), true);
//            EditorGUILayout.PropertyField(inputAnswers, new GUIContent("Input Answers"), true);
//            EditorGUILayout.PropertyField(inputBranches, new GUIContent("Branches"), true);
//        }
//        serializedObject.ApplyModifiedProperties();
//    }
//}

