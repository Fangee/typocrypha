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
        [ConnectionKnob("From Previous", Direction.In, "Gameflow", ConnectionCount.Multi, NodeSide.Left, 30)]
        public ConnectionKnob fromPreviousIN;

        [SerializeField]
        List<BranchCaseData> _cases;
        ReorderableList _list = null;

        private ConnectionKnobAttribute dynaCreationAttribute
            = new ConnectionKnobAttribute("To Branch Target", Direction.Out, "Gameflow", ConnectionCount.Single, NodeSide.Right);

        const string tooltip_branch_case = "A resizable list containing all branch conditions (temp)";

        protected override void OnCreate()
        {
            _cases = new List<BranchCaseData>();
            addListItem(_cases, 0);
        }

        public override void NodeGUI()
        {
            if (_list == null)
            {
                _list = new ReorderableList(_cases, typeof(BranchCaseData), true, true, false, false);
                _list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    if (index >= _list.list.Count)//Fixes error if .doGUI removes an element from the list
                        return;
                    BranchCaseData element = (BranchCaseData)_list.list[index];
                    listItemGUI(element, rect, index, _list.list);
                };
                _list.drawHeaderCallback = (Rect rect) => {
                    EditorGUI.LabelField(rect, new GUIContent("Branch Cases", tooltip_branch_case), new GUIStyle(GUIStyle.none) { alignment = TextAnchor.MiddleCenter });
                };
                _list.onReorderCallback = (ReorderableList list) =>
                {

                };
            }
            GUILayout.BeginVertical();
            _list.DoLayoutList();
            GUILayout.EndVertical();

        }

        private void listItemGUI(BranchCaseData item, Rect rect, int index, IList list)
        {
            if (!(Event.current.type == EventType.Repaint))//Only paint GUI during repaint (not layout)
                return;
            float xOffset = 0;
            Rect UIrect = new Rect(rect.x, rect.y + 2, 60, EditorGUIUtility.singleLineHeight);
            item.type = (BranchCaseData.CaseType)EditorGUI.EnumPopup(UIrect, GUIContent.none, item.type);

            xOffset += (UIrect.width + 2);
            UIrect = new Rect(rect.x + xOffset, rect.y + 1, 170, EditorGUIUtility.singleLineHeight);
            item.pattern = EditorGUI.TextField(UIrect, item.pattern);
            xOffset += (UIrect.width + 2);
            UIrect = new Rect(rect.x + xOffset, rect.y + 1, 20, EditorGUIUtility.singleLineHeight);
            if (list.Count > 1 && GUI.Button(UIrect, "‒"))
            {
                removeListItem(list, index);
                return;
            }
            xOffset += (UIrect.width + 2);
            UIrect = new Rect(rect.x + xOffset, rect.y + 1, 20, EditorGUIUtility.singleLineHeight);
            if (GUI.Button(UIrect, "+"))
            {
                //list.Insert(index + 1, new BranchCaseData());
                addListItem(list, index + 1);
            }
            ((ConnectionKnob)dynamicConnectionPorts[item.portIndex]).SetPosition(rect.yMax + NodeEditorGUI.knobSize / 2);
        }

        private void addListItem(IList list, int index)
        {
            CreateConnectionKnob(dynaCreationAttribute, index);
            BranchCaseData value = new BranchCaseData(index);
            list.Insert(index, value);
            CorrectNodeIndicesAfterInsert(list, index, value.portIndex);
        }

        private void CorrectNodeIndicesAfterInsert(IList list, int index, int portIndex)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                BranchCaseData b = (BranchCaseData)list[i];
                if (i != index && b.portIndex >= portIndex)
                    b.portIndex++;
            }
        }

        private void removeListItem(IList list, int index)
        {
            int portIndex = ((BranchCaseData)list[index]).portIndex;
            DeleteConnectionPort(portIndex);
            list.RemoveAt(index);
            CorrectNodeIndicesAfterRemove(list, portIndex);
        }

        private void CorrectNodeIndicesAfterRemove(IList list, int portIndex)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                BranchCaseData b = (BranchCaseData)list[i];
                if (b.portIndex > portIndex)
                    b.portIndex--;
            }
        }

        [System.Serializable]
        class BranchCaseData
        {
            public enum CaseType
            {
                Text,
                Regex
            }
            public BranchCaseData(int portIndex)
            {
                this.portIndex = portIndex;
            }  
            public string pattern =  string.Empty;
            public CaseType type = CaseType.Text;
            public int portIndex;
        }
    }
}
