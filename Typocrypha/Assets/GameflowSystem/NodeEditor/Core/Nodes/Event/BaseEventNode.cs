using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;

namespace TypocryphaGameflow
{

    [Node(true, "Event/Event Base", new System.Type[] { typeof(GameflowCanvas) })]
    public abstract class BaseEventNode : GameflowStandardIONode
    {
        public abstract void processEvent();
    }
}
