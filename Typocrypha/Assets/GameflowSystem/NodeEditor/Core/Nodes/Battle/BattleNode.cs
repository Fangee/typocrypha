﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;

namespace TypocryphaGameflow
{
    [Node(true, "Battle/BattleBase", new System.Type[] { typeof(GameflowCanvas) })]
    public abstract class BattleNode : BaseNodeIO
    {
        public override bool AllowRecursion { get { return true; } }

        #region Game
        #endregion
    }
}