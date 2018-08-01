using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace TypocryphaGameflow
{
    public static partial class GUIUtilities
    {
        public abstract class ReorderableListItem : ScriptableObject
        {
            public abstract void doGUI(Rect rect, int index, IList list);
            public virtual float getHeight(int index) { return EditorGUIUtility.singleLineHeight; }
            public abstract IEnumerable<System.Type> Subtypes { get; }
            public virtual void doAddMenu(IList list)
            {
                var menu = new NodeEditorFramework.Utilities.GenericMenu();
                foreach (var type in Subtypes)
                {
                    string[] path = type.ToString().Split('.');
                    var name = path[path.Length - 1];
                    menu.AddItem(new GUIContent(name),
                    false, clickHandler, new MenuItemData(type, list));
                }
                menu.Show(Event.current.mousePosition + Event.current.delta);   
            }
            private void clickHandler(object obj)
            {
                var data = (MenuItemData)obj;
                data.list.Add(ScriptableObject.CreateInstance(data.type));
            }
        }
        public abstract class ReorderableListItemNodeConnection : ReorderableListItem
        {
            public int[] nodeIndices;
        }
        //Data class used to generate add context menus
        public class MenuItemData
        {
            //public string path;
            public System.Type type;
            public IList list;
            public MenuItemData(System.Type type, IList list )
            {
                this.type = type;
                this.list = list;
            }
        }
        //Generic Reorderable list. Should only be used with non-polymorphic types marked with the System.Serializable
        public class ReorderableList<T>
        {
            public delegate void listItemGUI(T item, Rect rect, int index, IList list);
            public delegate float elementHeightCalc(T item, int index);
            public delegate void dropdownMenu();

            public List<T> elements;
            protected UnityEditorInternal.ReorderableList _list;
            private listItemGUI doItemGUI;
            private elementHeightCalc calcHeight = (item, index) => { return EditorGUIUtility.singleLineHeight; };
            private dropdownMenu doAddMenu;

            public ReorderableList() { }
            public ReorderableList(List<T> elements, listItemGUI elementGUIFn, elementHeightCalc heightFn, GUIContent headerLabel, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton)
            {
                this.elements = elements;
                this.doItemGUI = elementGUIFn;
                if (heightFn != null)
                    calcHeight = heightFn;
                _list = new UnityEditorInternal.ReorderableList(elements, typeof(T), draggable, displayHeader, displayAddButton, displayRemoveButton);
                _list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    if (index >= _list.list.Count)//Fixes error if .doGUI removes an element from the list
                        return;
                    T element = (T)_list.list[index];
                    doItemGUI(element, rect, index, _list.list);
                };
                _list.elementHeightCallback = (index) => { return calcHeight(elements[index], index); };
                _list.drawHeaderCallback = (Rect rect) => {
                    EditorGUI.LabelField(rect, headerLabel, new GUIStyle(GUIStyle.none) { alignment = TextAnchor.MiddleCenter });
                };
                _list.drawElementBackgroundCallback = (rect, index, active, focused) => {
                    rect.height = calcHeight(elements[index], index);
                    Texture2D tex = new Texture2D(1, 1);
                    tex.SetPixel(0, 0, new Color(0.33f, 0.66f, 1f, 0.66f));
                    tex.Apply();
                    if (active)
                        GUI.DrawTexture(rect, tex as Texture);
                };
                _list.onCanRemoveCallback = (ReorderableList l) => {
                    return l.count > 1;
                };
            }
            public ReorderableList(List<T> elements, listItemGUI elementGUIFn, elementHeightCalc heightFn, string label, string tooltip, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton) 
                    : this(elements, elementGUIFn, heightFn, new GUIContent(label, tooltip), draggable, displayHeader, displayAddButton, displayRemoveButton)
            {

            }
            public ReorderableList(List<T> elements, listItemGUI elementGUIFn, elementHeightCalc heightFn, GUIContent headerLabel, dropdownMenu addMenuFn, bool draggable, bool displayHeader)
                : this(elements, elementGUIFn, heightFn, headerLabel, draggable, displayHeader, true, true)
            {
                this.doAddMenu = addMenuFn;
                _list.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => { doAddMenu(); };
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
        //Specific reorderablelist implementation to be used with polymorphic scriptable objects that inherit from ReorderableListItem
        public class ReorderableItemList : ReorderableList<ReorderableListItem>
        {
            public ReorderableItemList(List<ReorderableListItem> elements, bool draggable = true, bool displayHeader = false, GUIContent headerLabel = null, bool displayAddButton = true, bool displayRemoveButton = true)
            {
                _list = new UnityEditorInternal.ReorderableList(elements, typeof(ReorderableListItem), draggable, displayHeader, displayAddButton, displayRemoveButton);
                _list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    if (index >= _list.list.Count)//Fixes error if .doGUI removes an element from the list
                        return;
                    ReorderableListItem element = (ReorderableListItem)_list.list[index];
                    element.doGUI(rect, index, elements);
                };
                _list.elementHeightCallback = (index) => { return elements[index].getHeight(index); };
                _list.drawHeaderCallback = (Rect rect) => {
                    EditorGUI.LabelField(rect, headerLabel, new GUIStyle(GUIStyle.none) { alignment = TextAnchor.MiddleCenter });
                };
                _list.drawElementBackgroundCallback = (rect, index, active, focused) => {
                    rect.height = elements[index].getHeight(index);
                    Texture2D tex = new Texture2D(1, 1);
                    tex.SetPixel(0, 0, new Color(0.33f, 0.66f, 1f, 0.66f));
                    tex.Apply();
                    if (active)
                        GUI.DrawTexture(rect, tex as Texture);
                };
                _list.onCanRemoveCallback = (ReorderableList l) => {
                    return l.count > 1;
                };
                if (displayAddButton)
                {
                    _list.onAddDropdownCallback = (buttonRect, list) =>
                    {
                        elements[0].doAddMenu(elements);
                    };
                }
            }
        }
    }
}
