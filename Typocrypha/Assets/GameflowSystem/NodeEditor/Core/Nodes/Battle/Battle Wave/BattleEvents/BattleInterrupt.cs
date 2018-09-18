﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeEditorFramework;

namespace TypocryphaGameflow
{
    public class BattleInterrupt : BattleNodeWave.BattleEvent
    {
        private ConnectionKnobAttribute[] _attr = { KnobAttributeOUT };
        public override ConnectionKnobAttribute[] KnobAttributes { get { return _attr; } }
        protected override GUIContent TitleLabel { get { return new GUIContent("Battle Interrupt", "TODO: Tooltip"); } }

        public override bool processEvent(BattleField field, BattleDataTracker battleData)
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
