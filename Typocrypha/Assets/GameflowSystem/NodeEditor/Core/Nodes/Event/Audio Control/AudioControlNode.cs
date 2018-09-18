using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeEditorFramework;

using System;
using TypocryphaGameflow.GUIUtilities;

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
        List<EventData> _events;
        public ReorderableSOList<EventData> events = null;
        protected override void OnCreate()
        {
            _events = new List<EventData>();
        }

        public override ScriptableObject[] GetScriptableObjects()
        {
            return _events.ToArray();
        }

        protected override void CopyScriptableObjects(Func<ScriptableObject, ScriptableObject> replaceSO)
        {
            for (int i = 0; i < _events.Count; ++i)
                _events[i] = (EventData)replaceSO(_events[i]);
        }

        public override void NodeGUI()
        {
            if (events == null)
                events = new ReorderableSOList<EventData>(_events, true, true, new GUIContent("Events", ""));
            GUILayout.Space(5);
            GUILayout.BeginVertical("box");
            events.doLayoutList();
            GUILayout.EndVertical();
        }

        public override ProcessFlag process(GameManagers managers)
        {
            throw new NotImplementedException();
        }

        public abstract class EventData : ReorderableSOList<EventData>.ListItem
        {
            public override float Height
            {
                get
                {
                    return EditorGUIUtility.singleLineHeight + 1;
                }
            }
        }
    }
}
