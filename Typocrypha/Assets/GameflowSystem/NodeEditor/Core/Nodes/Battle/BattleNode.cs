using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;

namespace TypocryphaGameflow
{
    [Node(true, "Dialog/DialogBase", new System.Type[] { typeof(GameflowCanvas) })]
    public abstract class BattleNode : BaseNodeIO
    {

        public override bool AutoLayout { get { return true; } }
        public override bool AllowRecursion { get { return true; } }

        #region Game
        public override BaseNode next()
        {
            return toNextOUT.connections[0].body as BaseNode;
        }
        public override ProcessFlag process()
        {
            //    player_ui.SetActive(false);
            DialogueManager.main.setEnabled(false);
            //BattleManagerS.main.startBattle(this);
            return ProcessFlag.Wait;
        }

        #endregion
    }
}