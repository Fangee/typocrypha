using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using NodeEditorFramework;
using TypocryphaGameflow.GUIUtilities;
using TypocryphaGameflow.MathUtils;

namespace TypocryphaGameflow
{
    [Node(false, "Gameflow/Branch", new System.Type[] { typeof(GameflowCanvas) })]
    public class GameflowBranchNode : BaseNode
    {
        #region Editor
        public enum controlExpressionType
        {
            Variable,
            Last_Input,
        }

        public override string Title { get { return "Gameflow Branch"; } }
        public override Vector2 MinSize { get { return new Vector2(300, 40); } }

        private const string Id = "gameflowBranch";
        public override string GetID { get { return Id; } }

        //Connection from previous node (INPUT)
        [ConnectionKnob("From Previous", Direction.In, "Gameflow", ConnectionCount.Multi, NodeSide.Left, 30)]
        public ConnectionKnob fromPreviousIN;
        //Connect to default branch (OUTPUT)
        [ConnectionKnob("To Default Branch", Direction.Out, "Gameflow", ConnectionCount.Single, NodeSide.Right, 30)]
        public ConnectionKnob toDefaultBranch;

        [SerializeField]
        List<BranchCaseData> _cases;
        ReorderableListConnectionKnob<BranchCaseData> _list = null;

        public controlExpressionType exprType;
        public string variableName;

        const string tooltip_branch_case = "The first case to evaluate to true (in decending order) will branch to the connected node. \nNote: macro variables may be evaluated in cases by using {varName} in text cases and \\{varName\\} in regex cases";
        const string tooltip_data = "Where to get the input string from. \"Input\" takes input from the last player input and \"Variable\" takes input from the variable with the given name";
        const string tooltip_branch_default = "The branch to take if no cases match";

        protected override void OnCreate()
        {
            _cases = new List<BranchCaseData>();
            _list = new ReorderableListConnectionKnob<BranchCaseData>(this, _cases, true, true, new GUIContent("Branch Cases", tooltip_branch_case), false, false);
            _list.AddItem(this, 0);
            exprType = controlExpressionType.Last_Input;
            variableName = "variable-name";
        }

        public override void NodeGUI()
        {
            if (_list == null)
            {
                _list = new ReorderableListConnectionKnob<BranchCaseData>(this,_cases, true, true, new GUIContent("Branch Cases", tooltip_branch_case), false, false);
            }
            GUILayout.Space(5);
            EditorGUILayout.BeginVertical("Box");
            GUILayout.Label(new GUIContent("Data to Test", tooltip_data), NodeEditorGUI.nodeLabelBoldCentered);
            GUILayout.BeginHorizontal();
            exprType = (controlExpressionType)EditorGUILayout.EnumPopup(exprType, GUILayout.Width(80f));
            if(exprType == controlExpressionType.Variable)
                variableName = EditorGUILayout.TextField("", variableName, GUILayout.Width(200f));
            GUILayout.Space(5);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            _list.doLayoutList();
            GUILayout.EndVertical();
            GUILayout.BeginVertical("Box");
            GUILayout.Label(new GUIContent("Default Branch", tooltip_branch_default), NodeEditorGUI.nodeLabelBoldCentered);
            toDefaultBranch.SetPosition();
            GUILayout.EndVertical();
        }

        #endregion

        #region Game
        public override BaseNode next()
        {
            string value = string.Empty;
            if (exprType == controlExpressionType.Last_Input)
                value = PlayerDataManager.main.LastPlayerInput;
            else
                value = PlayerDataManager.main.getData(variableName);
            foreach(var brCase in _cases)
            {
                if (brCase.type == BranchCaseData.CaseType.Regex)
                    throw new System.NotImplementedException();
                else if (checkTextCase(brCase.pattern, value))//brCase.type == BranchCaseData.CaseType.Text
                    return dynamicConnectionPorts[brCase.knobIndices.min].connection(0).body as BaseNode;
            }
            return toDefaultBranch.connection(0).body as BaseNode;
        }

        private bool checkTextCase(string pattern, string value)
        {
            //(old answer leniency: answerString = answer.Trim().ToLower().Replace(".", string.Empty).Replace("?", string.Empty).Replace("!", string.Empty);
            return value.Trim().ToLower() == DialogParser.main.substituteMacros(pattern).Trim().ToLower();
        }

        public override ProcessFlag process(GameManagers managers)
        {
            return ProcessFlag.Continue;
        }

        #endregion

        [System.Serializable]
        class BranchCaseData : ReorderableListItemConnectionKnob
        {
            public enum CaseType
            {
                Text,
                Regex
            }

            private static ConnectionKnobAttribute[] _knobAttributes = new ConnectionKnobAttribute[] { KnobAttributeOUT };
            public override ConnectionKnobAttribute[] KnobAttributes { get { return _knobAttributes; } }

            public string pattern =  string.Empty;
            public CaseType type = CaseType.Text;

            public override void doGUI(Rect rect, int index, AddItemFn addCallback, RmItemFn rmCallback)
            {
                float xOffset = 0;
                Rect UIrect = new Rect(rect.x, rect.y + 2, 60, EditorGUIUtility.singleLineHeight);
                type = (CaseType)EditorGUI.EnumPopup(UIrect, GUIContent.none, type);
                xOffset += (UIrect.width + 2);
                UIrect = new Rect(rect.x + xOffset, rect.y + 1, 170, EditorGUIUtility.singleLineHeight);
                pattern = EditorGUI.TextField(UIrect, pattern);
                xOffset += (UIrect.width + 2);
                UIrect = new Rect(rect.x + xOffset, rect.y + 1, 20, EditorGUIUtility.singleLineHeight);
                if (GUI.Button(UIrect, "‒"))
                {
                    rmCallback(index);
                    return;
                }
                xOffset += (UIrect.width + 2);
                UIrect = new Rect(rect.x + xOffset, rect.y + 1, 20, EditorGUIUtility.singleLineHeight);
                if (GUI.Button(UIrect, "+"))
                {
                    addCallback(index + 1);
                }
            }
        }
    }
}
