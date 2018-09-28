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

        [SerializeField] private List<BattleEvent> _events;
        public ReorderableSOList<BattleEvent> battleEvents;

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
            {
                ret.AddRange(e._conditions.ToArray());
                ret.AddRange(e._functions.ToArray());
            }
            return ret.ToArray();
        }

        protected override void CopyScriptableObjects(Func<ScriptableObject, ScriptableObject> replaceSO)
        {
            for (int i = 0; i < _events.Count; ++i)
            {
                List<BattleEvent.Condition> conditionObjs = new List<BattleEvent.Condition>();
                List<BattleEvent.Function> functionObjs = new List<BattleEvent.Function>();
                for (int j = 0; j < _events[i]._conditions.Count; ++j)
                {
                    ScriptableObject so = _events[i]._conditions[j];
                    so = replaceSO(so);
                    conditionObjs.Add(so as BattleEvent.Condition);
                }
                for(int j = 0; j <_events[i]._functions.Count; ++j)
                {
                    ScriptableObject so = _events[i]._functions[j];
                    so = replaceSO(so);
                    functionObjs.Add(so as BattleEvent.Function);
                }
                _events[i] = replaceSO(_events[i]) as BattleEvent;
                _events[i].node = this;
                _events[i]._conditions.Clear();
                _events[i]._conditions.AddRange(conditionObjs);
                _events[i]._functions.Clear();
                _events[i]._functions.AddRange(functionObjs);

            }
        }

        public override void NodeGUI()
        {
            if (battleEvents == null)
            {
                battleEvents = new ReorderableSOList<BattleEvent>(_events, true, true, new GUIContent("Battle Events", " "), true, false);
                battleEvents.processAddedItem = (item) =>
                {
                    item.node = this;
                };
            }

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
    }
}
