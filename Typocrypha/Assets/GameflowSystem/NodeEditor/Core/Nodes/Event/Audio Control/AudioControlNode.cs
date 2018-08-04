using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeEditorFramework;

using System;

namespace TypocryphaGameflow
{
    [Node(false, "Event/Audio Control", new System.Type[] { typeof(GameflowCanvas) })]
    public class AudioControlNode : BaseNodeIO
    {
        public const string ID = "Audio Control Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Audio Control"; } }
        public override Vector2 MinSize { get { return new Vector2(200, 20); } }

        [SerializeField]
        List<GUIUtilities.ReorderableListItem> _events;
        public GUIUtilities.ReorderableItemList events = null;
        protected override void OnCreate()
        {
            _events = new List<GUIUtilities.ReorderableListItem>();
            _events.Add((GUIUtilities.ReorderableListItem)ScriptableObject.CreateInstance(typeof(PlayBgm)));
        }

        public override ScriptableObject[] GetScriptableObjects()
        {
            return _events.ToArray();
        }

        protected override void CopyScriptableObjects(Func<ScriptableObject, ScriptableObject> replaceSO)
        {
            for (int i = 0; i < _events.Count; ++i)
                _events[i] = (GUIUtilities.ReorderableListItem)replaceSO(_events[i]);
        }

        public override void NodeGUI()
        {
            if (events == null)
                events = new GUIUtilities.ReorderableItemList(_events, true, true, new GUIContent("Events", ""));
            GUILayout.Space(5);
            GUILayout.BeginVertical("box");
            events.doLayoutList();
            GUILayout.EndVertical();
        }

        public override ProcessFlag process()
        {
            throw new NotImplementedException();
        }

        public abstract class EventData : GUIUtilities.ReorderableListItem
        {
            public static IEnumerable<System.Type> subtypes = GUIUtilities.ReflectiveEnumerator.GetAllSubclassTypes<EventData>();
            public override IEnumerable<Type> Subtypes { get { return subtypes; } }

            public override float getHeight(int index)
            {
                return EditorGUIUtility.singleLineHeight + 1;
            }
        }
    }
}
