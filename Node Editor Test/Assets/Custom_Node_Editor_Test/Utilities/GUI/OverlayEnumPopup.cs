using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;

using NodeEditorFramework.Utilities;

namespace TypocryphaGameflow
{
    public static partial class GUIUtilities
    {
        public static System.Enum OverlayEnumPopup(System.Enum target, Vector2 pos)
        {
            bool b = GUILayout.Button(System.Enum.GetName(target.GetType(), target));
            if (b)
            {
                Debug.Log("B");
                Debug.Log(Event.current);
            }

            if(Event.current.type == EventType.Layout && b)
            {
                GenericMenu popup = new GenericMenu();
                foreach (string s in System.Enum.GetNames(target.GetType()))
                    popup.AddItem(new GUIContent(s), false, null, null);
                popup.Show(pos);
                //popup.ShowAsContext();
                //Event.current.Use();
            }
            return target;
        }
    }
}


