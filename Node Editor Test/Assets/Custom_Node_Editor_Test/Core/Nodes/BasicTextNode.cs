using UnityEngine;
using UnityEditor;
using NodeEditorFramework;

namespace TypocryphaDialogSystemTest
{
    [Node(false, "Dialog/Testing/Test Dialog Node")]//, new System.Type[]{typeof(NodeEditorFramework.Standard.CalculationCanvasType) })]
    public class BaseTextNode : Node
    {
        public const string ID = "Test Dialog Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Test Dialog Node"; } }
        public override Vector2 MinSize { get { return new Vector2(300, 60); } }
        public override bool AutoLayout { get { return true; } }
        public override bool AllowRecursion { get { return true; } }

        //Connection from previous node (INPUT)
        [ConnectionKnob("From Previous", Direction.In, "Gameflow", NodeSide.Left, 30)]
        public ConnectionKnob fromPreviousIN;
        //Next Node to go to (OUTPUT)
        [ConnectionKnob("To Next", Direction.Out, "Gameflow", NodeSide.Right, 30)]
        public ConnectionKnob toNextOUT;

        public string characterName;
        public string expression;
        public string dialogText;

        private Vector2 scroll;
        protected GUIStyle labelStyle = new GUIStyle();

        protected override void OnCreate()
        {
            characterName = "Character Name";
            expression = "Expression";
            dialogText = "Insert dialog text here";
            labelStyle.normal.textColor = Color.white;
        }

        public override void NodeGUI()
        {
            EditorGUILayout.BeginVertical("Box");
            //GUILayout.BeginHorizontal();
            //CharacterPotrait = (Sprite)EditorGUILayout.ObjectField(CharacterPotrait, typeof(Sprite), false, GUILayout.Width(65f), GUILayout.Height(65f));
            characterName = EditorGUILayout.TextField("", characterName);
            expression = EditorGUILayout.TextField("", expression);
            //GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(15);

            GUILayout.BeginHorizontal();

            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(100));
            EditorStyles.textField.wordWrap = true;
            dialogText = EditorGUILayout.TextArea(dialogText, GUILayout.ExpandHeight(true));
            EditorStyles.textField.wordWrap = false;
            EditorGUILayout.EndScrollView();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 90;
            //if (GUILayout.Button("►", GUILayout.Width(20)))
            //{
            //    if (SoundDialog)
            //        AudioUtils.PlayClip(SoundDialog);
            //}
            GUILayout.EndHorizontal();
        }
    }
    public class GameflowType : ConnectionKnobStyle //: IConnectionTypeDeclaration
    {
        public override string Identifier { get { return "Gameflow"; } }
        public override Color Color { get { return Color.cyan; } }
    }
}
