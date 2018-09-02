using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using TypocryphaGameflow;

namespace TypocryphaGameflow
{
    // Container for data about specific characters (read only)
    [CreateAssetMenu]
    [System.Serializable]
    public class CharacterData : ScriptableObject
    {
        public NameSet aliases; // Different aliases/names for this character
        public NameMap poses; // Different body poses
        public NameMap expressions; // Different facial expressions
        public NameMap codecs; // Different codec sprites
        public Sprite chat_icon; // Chat mode sprite
        public UnityEngine.AudioClip talk_sfx; // Talking sound effect
    }

    // Serializable wrapper for dictionaries
    [System.Serializable]
    public class NameMap : SerializableDictionary<string, Sprite> {}

    // Serializable wrapper for sets
    [System.Serializable]
    public class NameSet : SerializableSet<string> { }

    #region GUI
    // CharacterData inspector (read-only)
    [CustomEditor(typeof(CharacterData))]
    public class CharacterDataInspector : Editor
    {
        CharacterData data;

        public override void OnInspectorGUI()
        {
            data = target as CharacterData;
            GUILayout.Label("Character: " + data.name);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            if (data.aliases == null)
            {
                data.aliases = new NameSet();
            }
            NameSetGUI("Aliases", data.aliases);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            if (data.poses == null)
            {
                data.poses = new NameMap();
            }
            NameMapGUI("Poses", data.poses);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            if (data.expressions == null)
            {
                data.expressions = new NameMap();
            }
            NameMapGUI("Expressions", data.expressions);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            if (data.codecs == null)
            {
                data.codecs = new NameMap();
            }
            NameMapGUI("Codecs", data.codecs);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            data.chat_icon = EditorGUILayout.ObjectField("Chat Icon", data.chat_icon, typeof(Sprite), false) as Sprite;
            data.talk_sfx = EditorGUILayout.ObjectField("Talking SFX", data.talk_sfx, typeof(UnityEngine.AudioClip), false) as UnityEngine.AudioClip;
            if (GUI.changed)
            {
                EditorUtility.SetDirty(data);
            }

        }


        void NameSetGUI(string title, NameSet nameSet)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(title + ": " + nameSet._items.Count);
            if (GUILayout.Button("+"))
            {
                nameSet._items.Add(string.Empty);
            }
            GUILayout.EndHorizontal();
            EditorGUI.indentLevel = 1;
            GUILayout.BeginHorizontal();
            GUILayout.Label("    Names");
            GUILayout.EndHorizontal();
            int toDelete = -1; // Item to delete; -1 if none chosen
            int toAdd = -1; // Item after which to add; -1 if none chosen
            for (int i = 0; i < nameSet._items.Count; ++i)
            {
                GUILayout.BeginHorizontal();
                nameSet._items[i] = EditorGUILayout.TextField(nameSet._items[i]);
                if (GUILayout.Button("+"))
                {
                    toAdd = i;
                }
                if (GUILayout.Button("-"))
                {
                    toDelete = i;
                }
                GUILayout.EndHorizontal();
            }
            if (toAdd != -1)
            {
                nameSet._items.Insert(toAdd + 1, string.Empty);
            }
            if (toDelete != -1)
            {
                nameSet._items.RemoveAt(toDelete);
            }
            EditorGUI.indentLevel = 0;
        }

        void NameMapGUI(string title, NameMap nameMap)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(title + ": " + nameMap._keys.Count);
            if (GUILayout.Button("+"))
            {
                nameMap._keys.Add(string.Empty);
                nameMap._values.Add(null);
            }
            GUILayout.EndHorizontal();
            EditorGUI.indentLevel = 1;
            GUILayout.BeginHorizontal();
            GUILayout.Label("    Names");
            GUILayout.Label("Sprites");
            GUILayout.EndHorizontal();
            int toDelete = -1; // Item to delete; -1 if none chosen
            int toAdd = -1; // Item after which to add; -1 if none chosen
            for (int i = 0; i < nameMap._values.Count; ++i)
            {
                GUILayout.BeginHorizontal();
                nameMap._keys[i] = EditorGUILayout.TextField(nameMap._keys[i]);
                nameMap._values[i] = EditorGUILayout.ObjectField(nameMap._values[i], typeof(Sprite), false) as Sprite;
                if (GUILayout.Button("+"))
                {
                    toAdd = i;
                }
                if (GUILayout.Button("-"))
                {
                    toDelete = i;
                }
                GUILayout.EndHorizontal();
            }
            if (toAdd != -1)
            {
                nameMap._keys.Insert(toAdd + 1, string.Empty);
                nameMap._values.Insert(toAdd + 1, null);
            }
            if (toDelete != -1)
            {
                nameMap._keys.RemoveAt(toDelete);
                nameMap._values.RemoveAt(toDelete);
            }
            EditorGUI.indentLevel = 0;
        }
    }
    #endregion
}

