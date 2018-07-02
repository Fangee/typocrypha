using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using NodeEditorFramework;

namespace TypocryphaGameflow
{
    [Node(false, "Gameflow/Branch", new System.Type[] { typeof(GameflowCanvas) })]
    public class GameflowBranchNode : Node
    {
        public override string Title { get { return "Gameflow Branch"; } }
        public override Vector2 MinSize { get { return new Vector2(300, 40); } }
        public override bool AutoLayout { get { return true; } }

        private const string Id = "gameflowBranch";
        public override string GetID { get { return Id; } }

        //Connection from previous node (INPUT)
        [ConnectionKnob("From Previous", Direction.In, "Gameflow", NodeSide.Left, 30)]
        public ConnectionKnob fromPreviousIN;

        [SerializeField]
        List<BranchCaseData> _cases;
        ReorderableList _list = null;

        const string tooltip_branch_case = "A resizable list containing all branch conditions (temp)";

        protected override void OnCreate()
        {
            _cases = new List<BranchCaseData>();
            _cases.Add(new BranchCaseData());
        }

        public override void NodeGUI()
        {
            if (_list == null)
            {
                _list = new ReorderableList(_cases, typeof(BranchCaseData), true, true, false, false);
                _list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    BranchCaseData element = (BranchCaseData)_list.list[index];
                    element.doGUI(rect, index, _list.list);
                };
                _list.drawHeaderCallback = (Rect rect) => {
                    EditorGUI.LabelField(rect, new GUIContent("Branch Cases", tooltip_branch_case), new GUIStyle(GUIStyle.none) { alignment = TextAnchor.MiddleCenter });
                };
            }
            GUILayout.BeginVertical();
            _list.DoLayoutList();
            GUILayout.EndVertical();

        }

        [System.Serializable]
        class BranchCaseData
        {
            public enum CaseType
            {
                Text,
                Regex
            }
            [NonSerialized]
            public bool expandOptions = false;
            public string pattern =  string.Empty;
            public CaseType type = CaseType.Text;
            public void doGUI(Rect rect, int index, IList list)
            {
                float xOffset = 0;
                Rect UIrect = new Rect(rect.x, rect.y + 2, 60, EditorGUIUtility.singleLineHeight);
                type = (CaseType)EditorGUI.EnumPopup(UIrect, GUIContent.none, type);

                xOffset += (UIrect.width + 2);
                UIrect = new Rect(rect.x + xOffset, rect.y + 1, 170, EditorGUIUtility.singleLineHeight);
                pattern = EditorGUI.TextField(UIrect, pattern);
                xOffset += (UIrect.width + 2);
                UIrect = new Rect(rect.x + xOffset, rect.y + 1, 20, EditorGUIUtility.singleLineHeight);
                if (list.Count > 1 && GUI.Button(UIrect, "‒"))
                {
                    list.RemoveAt(index);
                    //DeleteConnectionPort(index);
                }
                xOffset += (UIrect.width + 2);
                UIrect = new Rect(rect.x + xOffset, rect.y + 1, 20, EditorGUIUtility.singleLineHeight);
                if (GUI.Button(UIrect, "+"))
                {
                    list.Insert(index + 1, new BranchCaseData());
                }
            }
        }
    }
}
