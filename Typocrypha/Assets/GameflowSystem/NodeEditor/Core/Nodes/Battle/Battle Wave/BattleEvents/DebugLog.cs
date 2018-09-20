using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TypocryphaGameflow
{
    public class DebugLog : BattleNodeWave.BattleEvent
    {
        public override float Height { get { return HeaderHeight + ConditionListHeight + lineHeight; } }
        private static GUIContent _titleLabel = new GUIContent("Debug Log", "Prints a string to the Unity console. Allows macro parsing w/ {}");
        protected override GUIContent TitleLabel { get { return _titleLabel; } }

        public string message;

        public override void doGUI(Rect rect)
        {
            Rect UIRect = doHeaderGUI(rect);
            GUI.Label(new Rect(UIRect) { width = 60 }, new GUIContent("Message", "The string to print. Allows macro parsing w/ {}"));
            message = GUI.TextField(new Rect(UIRect) { x = UIRect.x + 60, width = UIRect.width - 60 }, message);
            UIRect.y += lineHeight;
            doConditionListGUI(UIRect);
        }

        public override bool processEvent(BattleField field, BattleDataTracker battleData)
        {
            Debug.Log(message);
            return false;
        }
    }
}

