using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeEditorFramework;
using System;

using TypocryphaGameflow.GUIUtilities;

namespace TypocryphaGameflow
{
    [Node(false, "Battle/Wave", new System.Type[] { typeof(GameflowCanvas) })]
    public class BattleNodeWave : BaseNodeIO
    {
        public enum TransitionType
        {
            Normal,
        }

        public const string ID = "Battle Wave Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Battle Wave"; } }
        public override Vector2 MinSize { get { return new Vector2(250, 60); } }

        public string waveTitle;
        public AudioClip music;
        public string enemyL;
        public string enemyM;
        public string enemyR;
        public string allyL;
        public string allyR;

        [SerializeField]
        private List<BattleEvent> _events;
        public ReorderableSOListConnectionKnob<BattleEvent> battleEvents;

        protected override void OnCreate()
        {
            _events = new List<BattleEvent>();
            battleEvents = null;
        }

        public override ScriptableObject[] GetScriptableObjects()
        {
            List<ScriptableObject> ret = new List<ScriptableObject>();
            ret.AddRange(_events.ToArray());
            foreach (BattleEvent e in _events)
                ret.AddRange(e._conditions.ToArray());
            return ret.ToArray();
        }

        protected override void CopyScriptableObjects(Func<ScriptableObject, ScriptableObject> replaceSO)
        {
            for (int i = 0; i < _events.Count; ++i)
            {
                List<BattleEventCondition> sublist = new List<BattleEventCondition>();
                for (int j = 0; j < _events[i]._conditions.Count; ++j)
                {
                    ScriptableObject so = _events[i]._conditions[j];
                    so = replaceSO(so);
                    sublist.Add(so as BattleEventCondition);
                }
                _events[i] = (BattleEvent)replaceSO(_events[i]);
                _events[i]._conditions.Clear();
                _events[i]._conditions.AddRange(sublist);
            }
        }

        public override void NodeGUI()
        {
            if (battleEvents == null)
                battleEvents = new ReorderableSOListConnectionKnob<BattleEvent>(this, _events, true, true, new GUIContent("Battle Events", " "));
            battleEvents.doLayoutList();
        }

        protected override void OnAddConnection(ConnectionPort port, ConnectionPort connection)
        {
            base.OnAddConnection(port, connection);
        }


        #region Game
        public override ProcessFlag process(GameManagers managers)
        {
            throw new NotImplementedException();
        }
        #endregion

        public abstract class BattleEvent : ReorderableSOListConnectionKnob<BattleEvent>.ListItem
        {
            public enum ConditionOperator
            {
                AND,
                OR,
                Equation
            }
            public override float Height
            {
                get
                {
                    if(conditions == null)
                        conditions = new ReorderableSOList<BattleEventCondition>(_conditions, true, true, new GUIContent("Conditions", ""));
                    return conditions.Height + (lineHeight * 2);
                }
            }
            public override ConnectionKnobAttribute[] KnobAttributes { get { return new ConnectionKnobAttribute[0]; } }
            public override void SetConnectionKnobPositions(Node n, Rect rect) { return; }

            #region Eval Fn Map and Functions
            private delegate bool ConditionEvalFn(List<BattleEventCondition> conditions, BattleField field, BattleDataTracker battleData);
            private static Dictionary<ConditionOperator, ConditionEvalFn> EvalFnMap = new Dictionary<ConditionOperator, ConditionEvalFn>
            {
                {ConditionOperator.AND, ANDEval },
                {ConditionOperator.OR, OREval },
                {ConditionOperator.Equation, EquationEval }
            };
            private static bool ANDEval(List<BattleEventCondition> conditions, BattleField field, BattleDataTracker battleData)
            {
                foreach (var c in conditions)
                {
                    bool expr = c.EvaluateCondition(field, battleData);
                    if (c.invert ? expr : !expr)
                        return false;
                }
                return true;
            }
            private static bool OREval(List<BattleEventCondition> conditions, BattleField field, BattleDataTracker battleData)
            {
                foreach (var c in conditions)
                {
                    bool expr = c.EvaluateCondition(field, battleData);
                    if (c.invert ? !expr : expr)
                        return true;
                }
                return false;
            }
            private static bool EquationEval(List<BattleEventCondition> conditions, BattleField field, BattleDataTracker battleData)
            {
                throw new System.NotImplementedException("Equation Evaluation not yet implemented");
            }
            #endregion

            public ConditionOperator conditionOperator = ConditionOperator.AND;
            protected bool stopChecking = false;

            protected abstract GUIContent TitleLabel { get; }

            [SerializeField]
            public List<BattleEventCondition> _conditions = new List<BattleEventCondition>();
            public ReorderableSOList<BattleEventCondition> conditions = null;

            public abstract bool processEvent(BattleField field, BattleDataTracker battleData);
            public bool checkConditions(BattleField field, BattleDataTracker battleData)
            {
                if (!stopChecking && EvalFnMap[conditionOperator](_conditions, field, battleData))
                    return processEvent(field, battleData);
                return false;
            }

            public override void doGUI(Rect rect)
            {
                if (conditions == null)
                    conditions = new ReorderableSOList<BattleEventCondition>(_conditions, true, true, new GUIContent("Conditions", ""));
                Rect UIrect = new Rect(rect) { height = EditorGUIUtility.singleLineHeight };
                GUI.Label(UIrect, TitleLabel, NodeEditorGUI.nodeLabelCentered);
                UIrect.y += lineHeight;
                conditionOperator = (ConditionOperator)EditorGUI.EnumPopup(UIrect, conditionOperator);
                Rect ListRect = new Rect(rect) { y = UIrect.y + lineHeight, height = conditions.Height };
                conditions.doList(ListRect);
            }
        }

        public abstract class BattleEventCondition : ReorderableSOList<BattleEventCondition>.ListItem
        {
            public bool invert = false;
            public abstract bool EvaluateCondition(BattleField field, BattleDataTracker battleData);
        }
  
    }
}
