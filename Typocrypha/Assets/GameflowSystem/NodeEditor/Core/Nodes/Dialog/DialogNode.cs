using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;

namespace TypocryphaGameflow
{
    [Node(true, "Dialog/DialogBase", new System.Type[] { typeof(GameflowCanvas) })]
    public abstract class DialogNode : BaseNodeIO
    {

        public override bool AutoLayout { get { return true; } }
        public override bool AllowRecursion { get { return true; } }

        public string characterName;
        public string text;

        public override BaseNode next()
        {
            return toNextOUT.connections[0].body as BaseNode;
        }
        public override ProcessFlag process(GameManagers managers)
        {
            //    player_ui.SetActive(false);
            managers.battleManager.setEnabled(false);
            managers.dialogueManager.startDialogue(new DialogueItem(characterName, text));
            return ProcessFlag.Wait;
        }
    }
}
