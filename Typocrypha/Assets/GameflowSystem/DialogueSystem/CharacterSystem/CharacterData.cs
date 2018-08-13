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
        public override void OnInspectorGUI()
        {
            GUILayout.Label("Character: " + target.name);
        }
    }

    // Custom editor for CharacterData
    public class CharacterDataEditor : EditorWindow
    {
        public CharacterData data;

        [MenuItem("Window/Character Data Editor %#c")]
        static void Init()
        {
            EditorWindow.GetWindow(typeof(CharacterDataEditor));
        }

        void OnEnable()
        {
            if (EditorPrefs.HasKey("ObjectPath"))
            {
                string objectPath = EditorPrefs.GetString("ObjectPath");
                data = AssetDatabase.LoadAssetAtPath(objectPath, typeof(CharacterData)) as CharacterData;
            }
        }

        void OnGUI()
        {
            HeaderGUI();

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

        void HeaderGUI()
        {
            GUILayout.BeginHorizontal();
            if (data)
            {
                GUILayout.Label("Character: " + data.name);
            }
            else
            {
                GUILayout.Label("No Character Data Selected");
            }

            if (GUILayout.Button("Open Character Data"))
            {
                string absPath = EditorUtility.OpenFilePanel("Select Character Data", "", "");
                if (absPath.StartsWith(Application.dataPath))
                {
                    string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
                    data = AssetDatabase.LoadAssetAtPath(relPath, typeof(CharacterData)) as CharacterData;
                    if (data)
                    {
                        EditorPrefs.SetString("ObjectPath", relPath);
                    }
                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = data;
                }
            }
            GUILayout.EndHorizontal();
        }

        void NameSetGUI(string title, NameSet nameSet)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(title + ": " + nameSet._items.Count);
            if (GUILayout.Button("+"))
            {
                nameSet._items.Add(string.Empty);
            }
            if (nameSet._items.Count > 0 && GUILayout.Button("-"))
            {
                nameSet._items.RemoveAt(nameSet._items.Count - 1);
            }
            GUILayout.EndHorizontal();
            for (int i = 0; i < nameSet._items.Count; ++i)
            {
                GUILayout.BeginHorizontal();
                nameSet._items[i] = EditorGUILayout.TextField(nameSet._items[i]);
                GUILayout.EndHorizontal();
            }
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
            if (nameMap._keys.Count > 0 && GUILayout.Button("-"))
            {
                nameMap._keys.RemoveAt(nameMap._keys.Count - 1);
                nameMap._values.RemoveAt(nameMap._values.Count - 1);
            }
            GUILayout.EndHorizontal();
            for (int i = 0; i < nameMap._values.Count; ++i)
            {
                GUILayout.BeginHorizontal();
                nameMap._keys[i] = EditorGUILayout.TextField(nameMap._keys[i]);
                nameMap._values[i] = EditorGUILayout.ObjectField(nameMap._values[i], typeof(Sprite), false) as Sprite;
                GUILayout.EndHorizontal();
            }
        }
    }
    #endregion
}

