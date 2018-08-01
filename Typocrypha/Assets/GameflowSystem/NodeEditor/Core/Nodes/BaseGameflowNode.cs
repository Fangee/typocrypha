using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;

namespace TypocryphaGameflow
{
    [Node(true, "Gameflow/GameflowBase", new System.Type[] { typeof(GameflowCanvas) })]
    public abstract class BaseGameflowNode : Node
    {
        public abstract BaseGameflowNode next();
    }
    public class GameflowType : ConnectionKnobStyle //: IConnectionTypeDeclaration
    {
        public override string Identifier { get { return "Gameflow"; } }
        public override Color Color { get { return Color.cyan; } }
    }
}
