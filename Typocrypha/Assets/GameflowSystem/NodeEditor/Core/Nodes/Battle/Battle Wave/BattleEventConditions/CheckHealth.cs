using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeEditorFramework;

namespace TypocryphaGameflow
{

    public class CheckHealth : BattleNodeWave.BattleEventCondition
    {
        public override float Height { get { return lineHeight * 2; } }

        public int HealthBelow = 50;

        public override void doGUI(Rect rect)
        {
            Rect UIRect = new Rect(rect) { height = EditorGUIUtility.singleLineHeight };
            GUI.Label(UIRect, new GUIContent("Check Health", "TODO: Tooltip"), NodeEditorGUI.nodeLabelCentered);
            UIRect.y += lineHeight;
            int labelWidth = 100;
            GUI.Label(new Rect(UIRect) { width = labelWidth }, new GUIContent("Health Is Below", "TODO: Tooltip"), GUI.skin.label);
            HealthBelow = EditorGUI.IntField(new Rect(UIRect) { width = UIRect.width - labelWidth + 1, x = UIRect.x + labelWidth + 1}, HealthBelow);
        }

        public override bool EvaluateCondition(BattleField field, BattleDataTracker battleData)
        {
            return field.Player.Curr_hp < HealthBelow;
        }
    }

}
