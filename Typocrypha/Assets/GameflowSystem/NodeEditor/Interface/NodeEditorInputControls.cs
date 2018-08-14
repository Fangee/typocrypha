﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;

namespace TypocryphaGameflow
{
    public static class NodeEditorInputControls
    {
        #region Node Context Entries
        #endregion

        #region ConnectionKnog Context Entries
        [EventHandlerAttribute(EventType.MouseDown)]
        private static void HandleNodeConnectionContext(NodeEditorInputInfo inputInfo)
        {
            NodeEditorState state = inputInfo.editorState;
            if (inputInfo.inputEvent.button == 2 && state.focusedConnectionKnob != null)
            { // Middle-Clicked on a ConnectionKnob, handle editing
                Debug.Log("right clicked on a node connection: " + state.focusedConnectionKnob);
            }
        }
        #endregion
    }
}
