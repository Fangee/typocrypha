using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeEditorFramework;

using NodeEditorFramework.Utilities;
using System;

namespace TypocryphaGameflow
{
    [Node(false, "Event/Character Control", new System.Type[] { typeof(GameflowCanvas) })]
    public class CharacterControlNode : GameflowStandardIONode
    {
        public enum ControlType 
        {
            Add,
            Remove,
            Move,
            Set_Expression,
        }

        public const string ID = "Character Control Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Character Control"; } }
        public override Vector2 MinSize { get { return new Vector2(150, 20); } }

        [SerializeField]
        List<GUIUtilities.ReorderableListItem> _events;
        public GUIUtilities.ReorderableItemList events = null;
        protected override void OnCreate()
        {
            _events = new List<GUIUtilities.ReorderableListItem>();
            _events.Add((GUIUtilities.ReorderableListItem)ScriptableObject.CreateInstance(typeof(AddCharacter)));
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

        public abstract class EventData : GUIUtilities.ReorderableListItem
        {
            public static IEnumerable<System.Type> subtypes = GUIUtilities.ReflectiveEnumerator.GetAllSubclassTypes<EventData>();
            public string characterName = "name";
            protected int numElements = 1;
            public override void doAddMenu(IList list)
            {
                var menu = new NodeEditorFramework.Utilities.GenericMenu();
                foreach (var type in subtypes)
                {
                    string[] path = type.ToString().Split('.');
                    var name = path[path.Length - 1];
                    menu.AddItem(new GUIContent(name),
                    false, clickHandler, new GUIUtilities.MenuItemData(type, list));
                }
                menu.ShowAsContext();
            }
            private void clickHandler(object obj)
            {
                var data = (GUIUtilities.MenuItemData)obj;
                data.list.Add(ScriptableObject.CreateInstance(data.type));
            }
            public override float getHeight(int index)
            {
                return base.getHeight(index) * 2;
            }
        }
    }
}
