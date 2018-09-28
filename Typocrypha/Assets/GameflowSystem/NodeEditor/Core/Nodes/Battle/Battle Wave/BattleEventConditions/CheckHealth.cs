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
        private static GUIContent _titleLabel = new GUIContent("Check Health", "TODO: Tooltip");
        protected override GUIContent TitleLabel { get { return _titleLabel; } }

        public int HealthBelow = 50;

        public override void doGUI(Rect rect)
        {
            Rect UIRect = doHeaderGUI(rect);
            int labelWidth = 100;
            GUI.Label(new Rect(UIRect) { width = labelWidth }, new GUIContent("Health Is Below", "TODO: Tooltip"), GUI.skin.label);
            HealthBelow = EditorGUI.IntField(new Rect(UIRect) { width = UIRect.width - labelWidth + 1, x = UIRect.x + labelWidth + 1}, HealthBelow);
        }

        public override bool EvaluateCondition(Battlefield field, BattleDataTracker battleData)
        {
            return field.Player.Curr_hp < HealthBelow;
        }
    }

}
