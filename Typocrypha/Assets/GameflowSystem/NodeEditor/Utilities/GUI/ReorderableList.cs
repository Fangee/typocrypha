﻿using System.Collections;
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
        #region List Item Abstract Classes

        #region Scriptable Object Items
        //Inherit From this class to create a local abstract base type to inherit Reoderable List Items from
        //Only define Subtypes in the the local abstract base and inherit individual Items from the local abstract base
        public abstract class ReorderableListSOBase : ScriptableObject
        {
            public abstract void doGUI(Rect rect);
            public virtual float getHeight() { return EditorGUIUtility.singleLineHeight; }
        }
        public abstract class ReorderableListSOConnectionKnobBase : ReorderableListSOBase
        {
            public abstract MathUtils.IntRange KnobIndices { get; }
            public abstract ConnectionKnobAttribute[] KnobAttributes { get; }
            public virtual void SetConnectionKnobPositions(Node n, Rect rect)
            {
                ((ConnectionKnob)n.dynamicConnectionPorts[KnobIndices.min]).SetPosition(rect.yMax + NodeEditorGUI.knobSize / 2);
            }
        }
        #endregion

        #region Serializable Class  Items
        public abstract class ReorderableListItem
        {
            public virtual float Height { get { return EditorGUIUtility.singleLineHeight + 1; } }
            public abstract void doGUI(Rect rect);
        }
        public abstract class ReorderableListItemConnectionKnob
        {
            public delegate void AddItemFn(int index);
            public delegate void RmItemFn(int index);
            public virtual float Height { get { return EditorGUIUtility.singleLineHeight + 1; } }
            public abstract void doGUI(Rect rect, int index, AddItemFn addCallback, RmItemFn rmCallback);
            public abstract MathUtils.IntRange KnobIndices { get; }
            public abstract ConnectionKnobAttribute[] KnobAttributes { get; }
            public virtual void SetConnectionKnobPositions(Node n, Rect rect)
            {
                ((ConnectionKnob)n.dynamicConnectionPorts[KnobIndices.min]).SetPosition(rect.yMax + (NodeEditorGUI.knobSize / 2) + 3);
            }       
        }
        #endregion

        #endregion

        #region List Classes
        //Generic Reorderable list. 
        //T: Should only be used with non-polymorphic types marked with the System.Serializable attribute
        public class ReorderableList<T> where T: ReorderableListItem
        {
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
                    EditorGUI.LabelField(rect, headerLabel, new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
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
        }
        //Generic Reorderable list for list items that Have associated connection knobs. 
        //T: Should only be used with non-polymorphic types marked with the System.Serializable attribute and inheriting ReorderableListItemConnectionKnob
        public class ReorderableListConnectionKnob<T> where T : ReorderableListItemConnectionKnob
        {
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
                    if (_list.count <= 0)
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
                _list.onCanAddCallback = (list) =>
                {
                    return (list.displayAdd = (list.count <= 0));
                };
            }

            public void AddItem(Node node, int index)
            {
                T item = System.Activator.CreateInstance(typeof(T), index) as T;
                for(int i = item.KnobIndices.min; i <= item.KnobIndices.max; ++i)
                {
                    node.CreateConnectionKnob(item.KnobAttributes[i - item.KnobIndices.min], i);
                }
                _list.list.Insert(index, item);
                CorrectNodeIndicesAfterInsert(item);
            }
            private void CorrectNodeIndicesAfterInsert(T insertedValue)
            {
                MathUtils.IntRange range = insertedValue.KnobIndices;
                for (int i = 0; i < _list.count; ++i)
                {
                    T value = _list.list[i] as T;
                    if (value != insertedValue && value.KnobIndices.min >= range.max)
                        value.KnobIndices.shift(range.Count);
                }
            }
            public void removeItem(Node node, int index)
            {
                MathUtils.IntRange range = (_list.list[index] as T).KnobIndices;
                for(int i = 0; i < range.Count; ++i)
                    node.DeleteConnectionPort(range.min);
                _list.list.RemoveAt(index);
                CorrectNodeIndicesAfterRemove(range);
            }
            private void CorrectNodeIndicesAfterRemove(MathUtils.IntRange range)
            {
                for (int i = 0; i < _list.count; ++i)
                {
                    T value = _list.list[i] as T;
                    if (value.KnobIndices.min > range.max)
                        value.KnobIndices.shift(range.Count * -1);
                }
            }

            public void doLayoutList()
            {
                _list.DoLayoutList();
            }
            public void doList(Rect rect)
            {
                _list.DoList(rect);
            }
        }
        //Specific reorderablelist implementation to be used with polymorphic scriptable objects that inherit from an abstract base that inherits from ReorderableItemBase
        public class ReorderableSOList<T> where T: ReorderableListSOBase
        {
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
                _list.elementHeightCallback = (index) => { return elements[index].getHeight(); };
                _list.drawHeaderCallback = (Rect rect) => {
                    EditorGUI.LabelField(rect, headerLabel, new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
                };
                _list.drawElementBackgroundCallback = (rect, index, active, focused) => {
                    if (_list.count <= 0)
                        return;
                    rect.height = elements[index].getHeight();
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
                _list.list.Add(ScriptableObject.CreateInstance(data));
            }

            public void doLayoutList()
            {
                _list.DoLayoutList();
            }
            public void doList(Rect rect)
            {
                _list.DoList(rect);
            }
        }
        #endregion
    }
}
