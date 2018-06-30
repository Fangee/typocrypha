using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;

namespace TypocryphaGameflow
{
    [Node(false, "Gameflow/branch")]
    public class GameflowBranchNode : Node
    {
        public override string Title { get { return "Gameflow Branch"; } }
        public override Vector2 MinSize { get { return new Vector2(200, 40); } }
        public override bool AutoLayout { get { return true; } }

        private const string Id = "gameflowBranch";
        public override string GetID { get { return Id; } }

        //Connection from previous node (INPUT)
        [ConnectionKnob("From Previous", Direction.In, "Gameflow", NodeSide.Left, 30)]
        public ConnectionKnob fromPreviousIN;

        protected override void OnCreate()
        {
        }
    }
}
