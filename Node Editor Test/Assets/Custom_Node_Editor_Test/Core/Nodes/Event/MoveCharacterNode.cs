using UnityEngine;
using UnityEditor;
using NodeEditorFramework;

namespace TypocryphaGameflow
{
    [Node(false, "Event/Move Character", new System.Type[] { typeof(GameflowCanvas) })]
    public class MoveCharacterNode : GameflowStandardIONode
    {
        public enum MovementType
        {
            Set_Position,
            Fade_In,
            Fade_Out,
            Crossfade,
            Slide,
        }

        public const string ID = "Move Character Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Move Character"; } }
        public override Vector2 MinSize { get { return new Vector2(150, 40); } }

        public string characterName;
        public MovementType moveType;
        public Vector2 moveTo;
        float time;

        protected override void OnCreate()
        {
            characterName = "Character Name";
            moveType = MovementType.Set_Position;
            moveTo = new Vector2(0, 0);
            time = 1;
        }

        public override void NodeGUI()
        {
            GUILayout.Space(5);
            GUILayout.BeginVertical("box");
            characterName = GUILayout.TextField(characterName);
            moveType = (MovementType)EditorGUILayout.EnumPopup(moveType);
            if (moveType != MovementType.Set_Position)
                time = EditorGUILayout.FloatField(time);
            moveTo = EditorGUILayout.Vector2Field("", moveTo);
            GUILayout.EndVertical();
        }
    }

}
