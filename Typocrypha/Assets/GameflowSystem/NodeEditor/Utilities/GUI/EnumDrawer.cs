using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeEditorFramework;

using NodeEditorFramework.Utilities;

namespace TypocryphaGameflow
{
    namespace GUIUtilities
    {
        public static class EnumDrawer
        {
            public static System.Enum OverlayEnumPopup(System.Enum target)
            {
                bool b = EditorGUILayout.DropdownButton(new GUIContent(System.Enum.GetName(target.GetType(), target)), FocusType.Keyboard);
                if (b)
                {
                    Debug.Log(b);
                    Debug.Log(Event.current);
                    NodeEditorFramework.Utilities.GenericMenu popup = new NodeEditorFramework.Utilities.GenericMenu();
                    foreach (string s in System.Enum.GetNames(target.GetType()))
                        popup.AddItem(new GUIContent(s), false, null, null);
                    Rect rect = GUILayoutUtility.GetLastRect();
                    rect.position = popup.Position;
                    popup.DropDown(rect);
                    Debug.Log(popup.Position);
                    Debug.Log(GUILayoutUtility.GetLastRect());
                }

                if (b)
                {
                    ;
                    //popup.ShowAsContext();
                    //Event.current.Use();
                }
                return target;
            }
        }
    }
}


