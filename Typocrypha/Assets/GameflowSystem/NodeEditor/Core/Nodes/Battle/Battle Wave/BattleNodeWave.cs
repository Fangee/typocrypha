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
        public const string ID = "Battle Wave Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Battle Wave"; } }
        public override Vector2 MinSize { get { return new Vector2(300, 60); } }

        public string waveTitle;
        public AudioClip music;
        public EnemyData[] enemyData;
        public AllyData[] allyData;

        [SerializeField]
        private List<BattleEvent> _events;
        public ReorderableSOListConnectionKnob<BattleEvent> battleEvents;

        #region Tooltip Strings
        private static string tooltip_music = "Music to play. Leave as None to keep the previous music playing";
        #endregion

        protected override void OnCreate()
        {
            _events = new List<BattleEvent>();
            battleEvents = null;
            enemyData = new EnemyData[3];
            allyData = new AllyData[2];
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

            #region Wave Transition GUI
            GUILayout.Space(4);
            GUILayout.BeginVertical("box");
            GUILayout.Label(new GUIContent("Wave Transition", "TODO: Tooltip"), NodeEditorGUI.nodeLabelBoldCentered);
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Title"), GUI.skin.label, GUILayout.Width(50));
            waveTitle = GUILayout.TextField(waveTitle);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Music", tooltip_music), GUI.skin.label, GUILayout.Width(50));
            music = EditorGUILayout.ObjectField(music, typeof(AudioClip), false) as AudioClip;
            GUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            #endregion

            #region Battlefield Data GUI
            GUILayout.BeginVertical("box");
            GUILayout.Label(new GUIContent("Battlefield Data", "TODO: Tooltip"), NodeEditorGUI.nodeLabelBoldCentered);
            GUILayout.BeginHorizontal();
            float objHeight = EditorGUIUtility.singleLineHeight * 1.125f;
            //GUILayout.Label(new GUIContent("Enemy Data"), NodeEditorGUI.nodeLabelCentered, GUILayout.Height(objHeight));
            enemyData[0] = EditorGUILayout.ObjectField(enemyData[0], typeof(EnemyData), false, GUILayout.Height(objHeight)) as EnemyData;
            enemyData[1] = EditorGUILayout.ObjectField(enemyData[1], typeof(EnemyData), false, GUILayout.Height(objHeight)) as EnemyData;
            enemyData[2] = EditorGUILayout.ObjectField(enemyData[2], typeof(EnemyData), false, GUILayout.Height(objHeight)) as EnemyData;
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
            GUILayout.BeginHorizontal();
            //GUILayout.Label(new GUIContent("Ally Data"), NodeEditorGUI.nodeLabelCentered, GUILayout.Height(objHeight));
            allyData[0] = EditorGUILayout.ObjectField(allyData[0], typeof(AllyData), false, GUILayout.Height(objHeight)) as AllyData;
            GUILayout.Label(new GUIContent("Player"), NodeEditorGUI.nodeLabelCentered, GUILayout.Width(rect.width * 0.33f - 7), GUILayout.Height(objHeight));
            allyData[1] = EditorGUILayout.ObjectField(allyData[1], typeof(AllyData), false, GUILayout.Height(objHeight)) as AllyData;
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
            GUILayout.EndVertical();
            #endregion

            battleEvents.doLayoutList();
        }

        #region Game
        public override ProcessFlag process(GameManagers managers)
        {
            managers.waveManager.startWave(this);
            return ProcessFlag.Wait;
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
            public override ConnectionKnobAttribute[] KnobAttributes { get { return new ConnectionKnobAttribute[0]; } }
            public override void SetConnectionKnobPositions(Node n, Rect rect) { return; }

            #region Eval Fn Map and Functions
            private delegate bool ConditionEvalFn(List<BattleEventCondition> conditions, Battlefield field, BattleDataTracker battleData);
            private static Dictionary<ConditionOperator, ConditionEvalFn> EvalFnMap = new Dictionary<ConditionOperator, ConditionEvalFn>
            {
                {ConditionOperator.AND, ANDEval },
                {ConditionOperator.OR, OREval },
                {ConditionOperator.Equation, EquationEval }
            };
            private static bool ANDEval(List<BattleEventCondition> conditions, Battlefield field, BattleDataTracker battleData)
            {
                foreach (var c in conditions)
                {
                    bool expr = c.EvaluateCondition(field, battleData);
                    if (c.invert ? expr : !expr)
                        return false;
                }
                return true;
            }
            private static bool OREval(List<BattleEventCondition> conditions, Battlefield field, BattleDataTracker battleData)
            {
                foreach (var c in conditions)
                {
                    bool expr = c.EvaluateCondition(field, battleData);
                    if (c.invert ? !expr : expr)
                        return true;
                }
                return false;
            }
            private static bool EquationEval(List<BattleEventCondition> conditions, Battlefield field, BattleDataTracker battleData)
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

            public abstract bool processEvent(Battlefield field, BattleDataTracker battleData);
            public bool checkConditions(Battlefield field, BattleDataTracker battleData)
            {
                if (!stopChecking && EvalFnMap[conditionOperator](_conditions, field, battleData))
                    return processEvent(field, battleData);
                return false;
            }

            #region Helper GUI Methods and properties
            protected Rect doHeaderGUI(Rect UIRect)
            {
                Rect headerRect = new Rect(UIRect) { height = EditorGUIUtility.singleLineHeight };
                GUI.Label(headerRect, TitleLabel, NodeEditorGUI.nodeLabelCentered);
                headerRect.y += lineHeight;
                return headerRect;
            }
            protected Rect doConditionListGUI(Rect UIRect)
            {
                if (_conditions.Count > 1)
                {
                    conditionOperator = (ConditionOperator)EditorGUI.EnumPopup(UIRect, conditionOperator);
                    UIRect.y += lineHeight;
                }
                conditions.doList(new Rect(UIRect) { height = conditions.Height });
                return new Rect(UIRect) { y = UIRect.y + conditions.Height, height = EditorGUIUtility.singleLineHeight };
            }
            protected float HeaderHeight { get { return lineHeight; } }
            protected float ConditionListHeight
            {
                get
                {
                    if (conditions == null)
                        conditions = new ReorderableSOList<BattleEventCondition>(_conditions, true, true, new GUIContent("Conditions", ""));
                    return conditions.Height + (_conditions.Count > 1 ? lineHeight : 0);
                }
            }
            #endregion
        }

        public abstract class BattleEventCondition : ReorderableSOList<BattleEventCondition>.ListItem
        {
            public bool invert = false;
            protected abstract GUIContent TitleLabel { get; }
            public abstract bool EvaluateCondition(Battlefield field, BattleDataTracker battleData);
            protected Rect doHeaderGUI(Rect rect)
            {
                Rect UIRect = new Rect(rect) { height = EditorGUIUtility.singleLineHeight};
                GUI.Label(new Rect(UIRect) { width = EditorGUIUtility.labelWidth }, TitleLabel, NodeEditorGUI.nodeLabelBold);
                GUI.Label(new Rect(UIRect) { x = rect.x + (0.825f * rect.width), width = 30 }, new GUIContent("NOT", "TODO: Tooltip"), GUI.skin.label);
                invert = EditorGUI.Toggle(new Rect(UIRect) { x = rect.x + 30 + (0.825f * rect.width), width = 20 }, invert);
                UIRect.y += lineHeight;
                return UIRect;
            }
        }
  
    }
}
