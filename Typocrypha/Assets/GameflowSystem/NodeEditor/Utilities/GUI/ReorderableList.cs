using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace TypocryphaGameflow
{
    namespace GUIUtilities
    {
        #region List Classes

        #region Serializable Class Lists
        //Generic Reorderable list. 
        //T: Should only be used with non-polymorphic types marked with the System.Serializable attribute
        public class ReorderableList<T> where T: ReorderableList<T>.ListItem
        {
            public float Height { get { return _list.GetHeight(); } }
            protected UnityEditorInternal.ReorderableList _list;
            public ReorderableList(List<T> elements, bool draggable = true, bool displayHeader = false, GUIContent headerLabel = null, bool displayAddButton = true, bool displayRemoveButton = true)
            {
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
                    EditorGUI.LabelField(rect, headerLabel, new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
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
            }
            public void doLayoutList()
            {
                _list.DoLayoutList();
            }
            public void doList(Rect rect)
            {
                _list.DoList(rect);
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
        //Generic Reorderable list for list items that Have associated connection knobs. 
        //T: Should only be used with non-polymorphic types marked with the System.Serializable attribute and inheriting ReorderableListItemConnectionKnob
        public class ReorderableListConnectionKnob<T> where T : ReorderableListConnectionKnob<T>.ListItem
        {
            public float Height { get { return _list.GetHeight(); } }
            protected UnityEditorInternal.ReorderableList _list;
            public ReorderableListConnectionKnob(Node node, List<T> elements, bool draggable = true, bool displayHeader = false, GUIContent headerLabel = null, bool displayAddButton = true, bool displayRemoveButton = true)
            {
                _list = new UnityEditorInternal.ReorderableList(elements, typeof(T), draggable, displayHeader, displayAddButton, displayRemoveButton);
                _list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    if (index >= _list.count || _list.count <= 0)//Fixes error if .doGUI removes an element from the list
                        return;
                    T element = _list.list[index] as T;
                    element.doGUI(rect, index, (ind) => { AddItem(node, ind); }, (ind) => { removeItem(node, ind); } );
                    if (Event.current.type == EventType.Repaint)
                        element.SetConnectionKnobPositions(node, rect);
                };
                _list.elementHeightCallback = (index) => {
                    if (index >= _list.count || _list.count <= 0)//Fixes error if .doGUI removes an element from the list
                        return 0;
                    return elements[index].Height;
                };
                _list.drawHeaderCallback = (Rect rect) => {
                    EditorGUI.LabelField(rect, headerLabel, new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
                };
                _list.drawElementBackgroundCallback = (rect, index, active, focused) => {
                    if (index >= _list.count || _list.count <= 0)
                        return;
                    rect.height = elements[index].Height;
                    Texture2D tex = new Texture2D(1, 1);
                    tex.SetPixel(0, 0, new Color(0.33f, 0.66f, 1f, 0.66f));
                    tex.Apply();
                    if (active)
                        GUI.DrawTexture(rect, tex as Texture);
                };
                _list.onAddCallback = (ButtonRect) =>
                {
                    AddItem(node, _list.count);
                };
            }
            
            #region Adding And Removing Elements
            public void AddItem(Node node, int index)
            {
                T item = System.Activator.CreateInstance<T>();
                int knobsBeforeItem = getNumKnobsBeforeIndex(index);
                item.knobIndices = new MathUtils.IntRange(knobsBeforeItem, knobsBeforeItem + item.KnobAttributes.Length - 1);
                for(int i = item.knobIndices.min; i <= item.knobIndices.max; ++i)
                {
                    node.CreateConnectionKnob(item.KnobAttributes[i - item.knobIndices.min], i);
                }
                _list.list.Insert(index, item);
                CorrectNodeIndicesAfterInsert(item);
                _list.displayAdd = false;
            }
            private void CorrectNodeIndicesAfterInsert(T insertedValue)
            {
                MathUtils.IntRange range = insertedValue.knobIndices;
                for (int i = 0; i < _list.count; ++i)
                {
                    T value = _list.list[i] as T;
                    if (value != insertedValue && value.knobIndices.min >= range.min)
                        value.knobIndices.shift(range.Count);
                }
            }
            private int getNumKnobsBeforeIndex(int index)
            {
                return index <= 0 ? 0 : index * (_list.list[0] as T).knobIndices.Count;
            }
            public void removeItem(Node node, int index)
            {
                MathUtils.IntRange range = (_list.list[index] as T).knobIndices;
                for(int i = 0; i < range.Count; ++i)
                    node.DeleteConnectionPort(range.min);
                _list.list.RemoveAt(index);
                CorrectNodeIndicesAfterRemove(range);
                _list.displayAdd = _list.count <= 0;
            }
            private void CorrectNodeIndicesAfterRemove(MathUtils.IntRange range)
            {
                for (int i = 0; i < _list.count; ++i)
                {
                    T value = _list.list[i] as T;
                    if (value.knobIndices.min > range.max)
                        value.knobIndices.shift(range.Count * -1);
                }
            }
            #endregion

            public void doLayoutList()
            {
                _list.DoLayoutList();
            }
            public void doList(Rect rect)
            {
                _list.DoList(rect);
            }

            #region List Item
            public abstract class ListItem
            {
                #region Standard ConnectionKnob Attributes
                protected static ConnectionKnobAttribute KnobAttributeOUT = new ConnectionKnobAttribute("OUT", Direction.Out, "Gameflow", ConnectionCount.Single, NodeSide.Right);
                protected static ConnectionKnobAttribute KnobAttributeIN = new ConnectionKnobAttribute("IN", Direction.In, "Gameflow", ConnectionCount.Multi, NodeSide.Left);
                #endregion

                #region Delegates
                public delegate void AddItemFn(int index);
                public delegate void RmItemFn(int index);
                #endregion

                protected static float lineHeight = EditorGUIUtility.singleLineHeight + 1;

                public MathUtils.IntRange knobIndices;
                public abstract ConnectionKnobAttribute[] KnobAttributes { get; }
                public abstract float Height { get; }
                public abstract void doGUI(Rect rect, int index, AddItemFn addCallback, RmItemFn rmCallback);
                public abstract void SetConnectionKnobPositions(Node n, Rect rect);
            }
            #endregion
        }
        #endregion

        #region Scriptable Object Class Lists
        //Specific reorderablelist implementation to be used with polymorphic scriptable objects that inherit from an abstract base that inherits from ReorderableListSOBase
        public class ReorderableSOList<T> where T: ReorderableSOList<T>.ListItem
        {
            public float Height { get { return _list.GetHeight(); } }
            protected UnityEditorInternal.ReorderableList _list;
            private IEnumerable _subtypes;
            public ReorderableSOList(List<T> elements, bool draggable = true, bool displayHeader = false, GUIContent headerLabel = null, bool displayAddButton = true, bool displayRemoveButton = true)
            {
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
                    EditorGUI.LabelField(rect, headerLabel, new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
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
                    _subtypes = ReflectiveEnumerator.GetAllSubclassTypes<T>();
                    _list.onAddDropdownCallback = (buttonRect, list) =>
                    {
                        doAddMenu();
                    };
                }
            }
            private void doAddMenu()
            {
                var menu = new NodeEditorFramework.Utilities.GenericMenu();
                foreach (var type in _subtypes)
                {
                    string[] path = type.ToString().Split('.');
                    var name = path[path.Length - 1];
                    menu.AddItem(new GUIContent(name), false, clickHandler, type);
                }
                menu.Show(Event.current.mousePosition + Event.current.delta);
            }
            private void clickHandler(object obj)
            {
                var data = (System.Type)obj;
                T item = ScriptableObject.CreateInstance(data) as T;
                _list.list.Add(item);
            }

            public void doLayoutList()
            {
                _list.DoLayoutList();
            }
            public void doList(Rect rect)
            {
                _list.DoList(rect);
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
        //Specific reorderablelist implementation to be used with polymorphic scriptable objects that inherit from an abstract base that inherits from ReorderableListSOBaseConnectionKnob
        public class ReorderableSOListConnectionKnob<T> where T : ReorderableSOListConnectionKnob<T>.ListItem
        {
            public float Height { get { return _list.GetHeight(); } }
            protected UnityEditorInternal.ReorderableList _list;
            private IEnumerable _subtypes;
            public ReorderableSOListConnectionKnob(Node node, List<T> elements, bool draggable = true, bool displayHeader = false, GUIContent headerLabel = null, bool displayAddButton = true, bool displayRemoveButton = true)
            {
                _list = new UnityEditorInternal.ReorderableList(elements, typeof(T), draggable, displayHeader, displayAddButton, displayRemoveButton);
                _list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    if (index >= _list.count || _list.count <= 0)//Fixes error if .doGUI removes an element from the list
                        return;
                    T element = _list.list[index] as T;
                    element.doGUI(rect);
                    if (Event.current.type == EventType.Repaint)
                        element.SetConnectionKnobPositions(node, rect);
                };
                _list.elementHeightCallback = (index) => { return elements[index].Height; };
                _list.drawHeaderCallback = (Rect rect) => {
                    EditorGUI.LabelField(rect, headerLabel, new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
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
                    _subtypes = ReflectiveEnumerator.GetAllSubclassTypes<T>();
                    _list.onAddDropdownCallback = (buttonRect, list) =>
                    {
                        doAddMenu(node);
                    };
                }
                _list.onRemoveCallback = (list) =>
                {
                    removeItem(node, list.index);
                };
            }
            private void doAddMenu(Node toAddTo)
            {
                var menu = new NodeEditorFramework.Utilities.GenericMenu();
                foreach (var type in _subtypes)
                {
                    string[] path = type.ToString().Split('.');
                    var name = path[path.Length - 1];
                    menu.AddItem(new GUIContent(name), false, clickHandler, new MenuData((System.Type)type, toAddTo));
                }
                menu.Show(Event.current.mousePosition + Event.current.delta);
            }
            private void clickHandler(object obj)
            {
                var data = (MenuData)obj;
                AddItem(data.addTo, data.type, _list.count);
            }

            #region Adding And Removing Elements
            public void AddItem(Node node, System.Type type, int index)
            {
                T item = ScriptableObject.CreateInstance(type) as T;
                if(item.KnobAttributes.Length > 0)
                {
                    int knobsBeforeItem = getNumKnobsBeforeIndex(index);
                    item.knobIndices = new MathUtils.IntRange(knobsBeforeItem, knobsBeforeItem + item.KnobAttributes.Length - 1);
                    for (int i = item.knobIndices.min; i <= item.knobIndices.max; ++i)
                    {
                        node.CreateConnectionKnob(item.KnobAttributes[i - item.knobIndices.min], i);
                    }
                    _list.list.Insert(index, item);
                    CorrectNodeIndicesAfterInsert(item);
                }
                else
                {
                    item.knobIndices = new MathUtils.IntRange(-1);
                    _list.list.Insert(index, item);
                }
            }
            private void CorrectNodeIndicesAfterInsert(T insertedValue)
            {
                MathUtils.IntRange range = insertedValue.knobIndices;
                for (int i = 0; i < _list.count; ++i)
                {
                    T value = _list.list[i] as T;
                    if (value != insertedValue && value.knobIndices.min >= range.min)
                        value.knobIndices.shift(range.Count);
                }
            }
            private int getNumKnobsBeforeIndex(int index)
            {
                if (index == 0)
                    return 0;
                int count = 0;
                for (int i = 0; i < index; ++i)
                {
                    count += (_list.list[i] as T).knobIndices.Count;
                }
                return count;
            }
            public void removeItem(Node node, int index)
            {
                MathUtils.IntRange knobIndices = (_list.list[index] as T).knobIndices;
                if (knobIndices.Count > 0)
                {
                    for (int i = 0; i < knobIndices.Count; ++i)
                        node.DeleteConnectionPort(knobIndices.min);
                    _list.list.RemoveAt(index);
                    CorrectNodeIndicesAfterRemove(knobIndices);
                }
                else
                    _list.list.RemoveAt(index);

            }
            private void CorrectNodeIndicesAfterRemove(MathUtils.IntRange range)
            {
                for (int i = 0; i < _list.count; ++i)
                {
                    T value = _list.list[i] as T;
                    if (value.knobIndices.min > range.max)
                        value.knobIndices.shift(range.Count * -1);
                }
            }
            #endregion

            public void doLayoutList()
            {
                _list.DoLayoutList();
            }
            public void doList(Rect rect)
            {
                _list.DoList(rect);
            }

            protected class MenuData
            {
                public System.Type type;
                public Node addTo;
                public MenuData(System.Type type, Node addTo)
                {
                    this.type = type;
                    this.addTo = addTo;
                }
            }

            #region List Item
            //Inherit From this class to create a local abstract base type to inherit Reoderable List SO Items (with connection knobs) from
            //Only define Subtypes in the the local abstract base and inherit individual Items from the local abstract base
            public abstract class ListItem : ReorderableSOList<T>.ListItem
            {
                #region Standard ConnectionKnob Attributes
                protected static ConnectionKnobAttribute KnobAttributeOUT = new ConnectionKnobAttribute("OUT", Direction.Out, "Gameflow", ConnectionCount.Single, NodeSide.Right);
                protected static ConnectionKnobAttribute KnobAttributeIN = new ConnectionKnobAttribute("IN", Direction.In, "Gameflow", ConnectionCount.Multi, NodeSide.Left);
                protected static void SetSingleConnectionKnob(ListItem item, Node n, Rect rect)
                {
                    ((ConnectionKnob)n.dynamicConnectionPorts[item.knobIndices.min]).SetPosition(rect.yMin + (NodeEditorGUI.knobSize * 2) - 3);
                }
                #endregion

                public MathUtils.IntRange knobIndices;
                public abstract ConnectionKnobAttribute[] KnobAttributes { get; }
                public abstract void SetConnectionKnobPositions(Node n, Rect rect);
            }
            #endregion
        }
        #endregion

        #endregion
    }
}
