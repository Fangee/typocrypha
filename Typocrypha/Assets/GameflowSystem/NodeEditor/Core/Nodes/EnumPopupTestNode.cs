using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;

namespace Gameflow
{
    [Node(true, "Event/Enum Test", new System.Type[] { typeof(GameflowCanvas) })]
    public class EnumPopupTestNode : Node
    {
        public const string ID = "Enum Test Node";
        public override string GetID { get { return ID; } }
        public override string Title { get { return "Enum Test"; } }
        public override bool AllowRecursion { get { return true; } }
        public override bool AutoLayout { get { return true; } }


        public enum Test
        {
            ONE, TWO, MIC, CHECK,

        }
        public Test test;
        protected override void OnCreate()
        {
            test = Test.MIC;
        }
        public override void NodeGUI()
        {
            test = (Test)GUIUtilities.EnumDrawer.OverlayEnumPopup(test);
        }
    }
}
