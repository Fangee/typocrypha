using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeEditorFramework;

namespace TypocryphaGameflow
{
    [Node(false, "Event/Character Control", new System.Type[] { typeof(GameflowCanvas) })]
    public class CharacterControlNode : GameflowStandardIONode
    {
        public enum ControlType 
        {
            Add,
            Remove,
            Move,
            Set_Expression,
        }

        public const string ID = "Character Control Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Character Control"; } }
        public override Vector2 MinSize { get { return new Vector2(150, 20); } }

        public ControlType controlType;
        public string characterName;
        public Vector2 pos;
        public string expression;
        public string startingExpression;
        public MoveCharacterNode.MovementType moveType;
        public Vector2 moveTo;
        float time;


        protected override void OnCreate()
        {
            controlType = ControlType.Add;
            characterName = "Character Name";
            startingExpression = "default";
            expression = "default";
            moveType = MoveCharacterNode.MovementType.Set_Position;
            moveTo = new Vector2(0, 0);
            time = 1;
            pos = new Vector2(0, 0);
        }

        public override void NodeGUI()
        {
            GUILayout.Space(5);
            GUILayout.BeginVertical("box");
            characterName = GUILayout.TextField(characterName);
            controlType = (ControlType)EditorGUILayout.EnumPopup(controlType);
            GUILayout.EndVertical();
            GUILayout.Space(1);
            GUILayout.BeginVertical("box");
            switch (controlType)
            {
                case ControlType.Add:
                    startingExpression = GUILayout.TextField(startingExpression);
                    pos = EditorGUILayout.Vector2Field("", pos);
                    break;
                case ControlType.Remove:
                    break;
                case ControlType.Move:
                    moveType = (MoveCharacterNode.MovementType)EditorGUILayout.EnumPopup(moveType);
                    if (moveType != MoveCharacterNode.MovementType.Set_Position)
                        time = EditorGUILayout.FloatField(time);
                    moveTo = EditorGUILayout.Vector2Field("", moveTo);
                    break;
                case ControlType.Set_Expression:
                    expression = GUILayout.TextField(expression);
                    break;
            }
            GUILayout.EndVertical();
        }

        //[System.Serializable]
        //public class EventData
        //{
        //    EventData(string [] args, string[] labels)
        //    {
        //        this.args = args;
        //        this.labels = labels;
        //    }
        //    public string[] args;
        //    public string[] labels;
        //}
    }
}
