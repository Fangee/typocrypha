using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using ReflectionUtils;

namespace GUIUtilities
{
    #region List Classes

    #region Serializable Class Lists
    //Generic Reorderable list. 
    //T: Should only be used with non-polymorphic types marked with the System.Serializable attribute
    public class ReorderableList<T> where T : ReorderableList<T>.ListItem
    {
        private bool expand = false;
        private GUIContent titleLabel;

        #region Callbacks
        public delegate void processAddedItemCallback(T addedItem);
        public processAddedItemCallback processAddedItem = (T) => { return; };
        #endregion

        public float Height { get { return expand ? _list.GetHeight() : EditorGUIUtility.singleLineHeight; } }
        protected UnityEditorInternal.ReorderableList _list;
        public ReorderableList(List<T> elements, bool draggable = true, bool displayHeader = false, GUIContent headerLabel = null, bool displayAddButton = true, bool displayRemoveButton = true)
        {
            titleLabel = displayHeader ? headerLabel : new GUIContent("");
            _list = new UnityEditorInternal.ReorderableList(elements, typeof(T), draggable, displayHeader, displayAddButton, displayRemoveButton);
            _list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (index >= _list.count || _list.count <= 0)//Fixes error if .doGUI removes an element from the list
                    return;
                T element = _list.list[index] as T;
                element.doGUI(rect);
            };
            _list.elementHeightCallback = (index) => { return elements[index].Height; };
            _list.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(new Rect(rect), titleLabel, new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
                Rect UIRect = new Rect(rect) { width = 50, x = rect.x + (expand ? 0 : 5) };
                expand = EditorGUI.ToggleLeft(UIRect, new GUIContent("Show"), expand);
            };
            _list.drawElementBackgroundCallback = (rect, index, active, focused) => {
                if (_list.count <= 0)
                    return;
                rect.height = elements[index].Height;
                Texture2D tex = new Texture2D(1, 1);
                tex.SetPixel(0, 0, new Color(0.33f, 0.66f, 1f, 0.66f));
                tex.Apply();
                if (active)
                    GUI.DrawTexture(rect, tex as Texture);
            };
            _list.onAddCallback = (list) =>
            {
                T item = System.Activator.CreateInstance<T>();
                processAddedItem(item);
                elements.Insert(elements.Count, item);
            };
        }
        public void doLayoutList()
        {
            if (expand)
                _list.DoLayoutList();
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(5);
                expand = EditorGUILayout.ToggleLeft(new GUIContent("Show", "TODO: Tooltip"), expand, GUILayout.Width(50));
                EditorGUILayout.LabelField(titleLabel, new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold });
                GUILayout.EndHorizontal();
            }
        }
        public void doList(Rect rect)
        {
            if (expand)
                _list.DoList(rect);
            else
                _list.drawHeaderCallback(rect);
        }

        #region List Item
        public abstract class ListItem
        {
            protected static float lineHeight = EditorGUIUtility.singleLineHeight + 1;
            public abstract float Height { get; }
            public abstract void doGUI(Rect rect);
        }
        #endregion
    }
    #endregion

    #region Scriptable Object Class Lists
    //Specific reorderablelist implementation to be used with polymorphic scriptable objects that inherit from an abstract base that inherits from ReorderableListSOBase
    public class ReorderableSOList<T> where T : ReorderableSOList<T>.ListItem
    {
        private bool expand = false;
        private GUIContent titleLabel;

        #region Callbacks
        public delegate void processAddedItemCallback(T addedItem);
        public processAddedItemCallback processAddedItem = (T) => { return; };
        #endregion

        public float Height { get { return expand ? _list.GetHeight() : EditorGUIUtility.singleLineHeight; } }
        protected UnityEditorInternal.ReorderableList _list;
        private IEnumerable _subtypes;
        public ReorderableSOList(List<T> elements, bool draggable = true, bool displayHeader = false, GUIContent headerLabel = null, bool displayAddButton = true, bool useDopdownMenu = true, bool displayRemoveButton = true)
        {
            titleLabel = headerLabel;
            _list = new UnityEditorInternal.ReorderableList(elements, typeof(T), draggable, displayHeader, displayAddButton, displayRemoveButton);
            _list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (index >= _list.count || _list.count <= 0)//Fixes error if .doGUI removes an element from the list
                    return;
                T element = _list.list[index] as T;
                element.doGUI(rect);
            };
            _list.elementHeightCallback = (index) => { return elements[index].Height; };
            _list.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(new Rect(rect), headerLabel, new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
                Rect UIRect = new Rect(rect) { width = 50, x = rect.x + (expand ? 0 : 5) };
                expand = EditorGUI.ToggleLeft(UIRect, new GUIContent("Show"), expand);
            };
            _list.drawElementBackgroundCallback = (rect, index, active, focused) => {
                if (_list.count <= 0)
                    return;
                rect.height = elements[index].Height;
                Texture2D tex = new Texture2D(1, 1);
                tex.SetPixel(0, 0, new Color(0.33f, 0.66f, 1f, 0.66f));
                tex.Apply();
                if (active)
                    GUI.DrawTexture(rect, tex as Texture);
            };
            if (displayAddButton)
            {
                if (useDopdownMenu)
                {
                    _subtypes = ReflectiveEnumerator.GetAllSubclassTypes<T>();
                    _list.onAddDropdownCallback = (buttonRect, list) =>
                    {
                        doAddMenu();
                    };
                }
                else
                {
                    _list.onAddDropdownCallback = (buttonRect, list) =>
                    {
                        T item = ScriptableObject.CreateInstance<T>();
                        processAddedItem(item);
                        elements.Insert(elements.Count, item);
                    };
                }
            }
        }
        private void doAddMenu()
        {
            var menu = new GenericMenu();
            foreach (var type in _subtypes)
            {
                string[] path = type.ToString().Split('.');
                var name = path[path.Length - 1];
                menu.AddItem(new GUIContent(name), false, clickHandler, type);
            }
            menu.DropDown(GUILayoutUtility.GetLastRect());
        }
        private void clickHandler(object obj)
        {
            var data = (System.Type)obj;
            T item = ScriptableObject.CreateInstance(data) as T;
            processAddedItem(item);
            _list.list.Add(item);
        }

        public void doLayoutList()
        {
            if (expand)
                _list.DoLayoutList();
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(5);
                expand = EditorGUILayout.ToggleLeft(new GUIContent("Show", "TODO: Tooltip"), expand, GUILayout.Width(50));
                EditorGUILayout.LabelField(titleLabel, new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold });
                GUILayout.EndHorizontal();
            }

        }
        public void doList(Rect rect)
        {
            if (expand)
                _list.DoList(rect);
            else
                _list.drawHeaderCallback(rect);
        }

        #region List Item
        //Inherit From this class to create a local abstract base type to inherit Reoderable List SO Items from
        //Only define Subtypes in the the local abstract base and inherit individual Items from the local abstract base
        public abstract class ListItem : ScriptableObject
        {
            protected static float lineHeight = EditorGUIUtility.singleLineHeight + 1;
            public abstract void doGUI(Rect rect);
            public abstract float Height { get; }
        }
        #endregion
    }
    #endregion

    #endregion
}
