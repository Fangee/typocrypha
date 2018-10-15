using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeEditorFramework;

namespace Gameflow
{
    public class BattleInterrupt : BattleEvent.Function
    {
        public override float Height { get { return lineHeight; } }
        private ConnectionKnobAttribute[] _attr = { KnobAttributeOUT };
        public override ConnectionKnobAttribute[] KnobAttributes { get { return _attr; } }
        private static GUIContent titleLabel = new GUIContent("Battle Interrupt", "TODO: Tooltip"); 

        public override void doGUI(Rect rect)
        {
            Rect UIRect = new Rect(rect) { height = EditorGUIUtility.singleLineHeight };
            GUI.Label(UIRect, titleLabel);
        }

        public override bool call(Battlefield field, BattleDataTracker battleData)
        {
            throw new System.NotImplementedException("Goto Battle Interrupt");
            return true;
        }
        public override void SetConnectionKnobPositions(Node n, Rect rect)
        {
            SetSingleConnectionKnob(this, n, rect);
        }
    }
}
