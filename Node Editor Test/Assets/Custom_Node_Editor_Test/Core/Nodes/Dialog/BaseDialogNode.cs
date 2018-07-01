using UnityEngine;
using NodeEditorFramework;

namespace TypocryphaGameflow
{
    [Node(true, "Dialog/Testing/Dialog Node Base", new System.Type[]{typeof(GameflowCanvas)})]
    public abstract class BaseDialogNode : Node
    {
        public override bool AllowRecursion { get { return true; } }

        public string characterName;
        public string dialogText;
    }
    public class GameflowType : ConnectionKnobStyle //: IConnectionTypeDeclaration
    {
        public override string Identifier { get { return "Gameflow"; } }
        public override Color Color { get { return Color.cyan; } }
    }
}
