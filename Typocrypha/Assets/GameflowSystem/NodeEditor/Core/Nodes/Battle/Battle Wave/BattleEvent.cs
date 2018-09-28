using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeEditorFramework;
using TypocryphaGameflow.GUIUtilities;

namespace TypocryphaGameflow
{
    public class BattleEvent : ReorderableSOList<BattleEvent>.ListItem
    {
        public enum ConditionOperator
        {
            AND,
            OR,
            Equation
        }

        #region Eval Fn Map and Functions
        private delegate bool ConditionEvalFn(List<Condition> conditions, Battlefield field, BattleDataTracker battleData);
        private static Dictionary<ConditionOperator, ConditionEvalFn> EvalFnMap = new Dictionary<ConditionOperator, ConditionEvalFn>
            {
                {ConditionOperator.AND, ANDEval },
                {ConditionOperator.OR, OREval },
                {ConditionOperator.Equation, EquationEval }
            };
        private static bool ANDEval(List<Condition> conditions, Battlefield field, BattleDataTracker battleData)
        {
            foreach (var c in conditions)
            {
                bool expr = c.EvaluateCondition(field, battleData);
                if (c.invert ? expr : !expr)
                    return false;
            }
            return true;
        }
        private static bool OREval(List<Condition> conditions, Battlefield field, BattleDataTracker battleData)
        {
            foreach (var c in conditions)
            {
                bool expr = c.EvaluateCondition(field, battleData);
                if (c.invert ? !expr : expr)
                    return true;
            }
            return false;
        }
        private static bool EquationEval(List<Condition> conditions, Battlefield field, BattleDataTracker battleData)
        {
            throw new System.NotImplementedException("Equation Evaluation not yet implemented");
        }
        #endregion

        public ConditionOperator conditionOperator = ConditionOperator.AND;
        protected bool stopChecking = false;

        private static GUIContent titleLabel = new GUIContent("Battle Event", "TODO: tooltip");

        [SerializeField] public List<Condition> _conditions = new List<Condition>();
        public ReorderableSOList<Condition> conditions = null;
        [SerializeField] public List<Function> _functions = new List<Function>();
        public ReorderableSOListConnectionKnob<Function> functions = null;
        public Node node;

        #region Game Function (NEEDS IMPLEMENTATION)
        public bool processEvents(Battlefield field, BattleDataTracker battleData)
        {
            return false;
        }
        public bool checkConditions(Battlefield field, BattleDataTracker battleData)
        {
            if (!stopChecking && EvalFnMap[conditionOperator](_conditions, field, battleData))
                return processEvents(field, battleData);
            return false;
        }
        #endregion

        public override void doGUI(Rect rect)
        {
            Rect UIRect = new Rect(rect) { height = EditorGUIUtility.singleLineHeight };
            GUI.Label(UIRect, titleLabel, NodeEditorGUI.nodeLabelCentered);
            UIRect.y += lineHeight;
            if (_conditions.Count > 1)
            {
                conditionOperator = (ConditionOperator)EditorGUI.EnumPopup(UIRect, conditionOperator);
                UIRect.y += lineHeight;
            }
            conditions.doList(new Rect(UIRect) { height = conditions.Height });
            UIRect.y += conditions.Height;
            functions.doList(new Rect(UIRect) { height = functions.Height });
        }
        public override float Height
        {
            get
            {
                if (conditions == null)
                    conditions = new ReorderableSOList<Condition>(_conditions, true, true, new GUIContent("Conditions", ""));
                if (functions == null)
                    functions = new ReorderableSOListConnectionKnob<Function>(node, _functions, true, true, new GUIContent("Functions", "TODO: tooltip"));
                return conditions.Height + functions.Height + lineHeight * (_conditions.Count > 1 ? 2 : 1);
            }
        }

        #region List Item Classes
        public abstract class Function : ReorderableSOListConnectionKnob<Function>.ListItem
        {
            public override ConnectionKnobAttribute[] KnobAttributes { get { return new ConnectionKnobAttribute[0]; } }
            public override void SetConnectionKnobPositions(Node n, Rect rect) { return; }

            public abstract bool call(Battlefield field, BattleDataTracker battleData);
        }

        public abstract class Condition : ReorderableSOList<Condition>.ListItem
        {
            public bool invert = false;
            protected abstract GUIContent TitleLabel { get; }
            public abstract bool EvaluateCondition(Battlefield field, BattleDataTracker battleData);
            protected Rect doHeaderGUI(Rect rect)
            {
                Rect UIRect = new Rect(rect) { height = EditorGUIUtility.singleLineHeight };
                GUI.Label(new Rect(UIRect) { width = EditorGUIUtility.labelWidth }, TitleLabel, NodeEditorGUI.nodeLabelBold);
                GUI.Label(new Rect(UIRect) { x = rect.x + (0.825f * rect.width), width = 30 }, new GUIContent("NOT", "TODO: Tooltip"), GUI.skin.label);
                invert = EditorGUI.Toggle(new Rect(UIRect) { x = rect.x + 30 + (0.825f * rect.width), width = 20 }, invert);
                UIRect.y += lineHeight;
                return UIRect;
            }
        }
        #endregion
    }
}
