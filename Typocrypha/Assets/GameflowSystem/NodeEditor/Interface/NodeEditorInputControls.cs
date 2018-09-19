using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
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

        #region Hotkeys
        [Hotkey(KeyCode.D, EventModifiers.Control)]
        private static void DeleteNode(NodeEditorInputInfo info)
        {
            NodeEditorState state = info.editorState;
            if (state.focusedNode != null)
                state.focusedNode.Delete();
        }

        #region Saving
        [Hotkey(KeyCode.S, EventModifiers.Control)]
        private static void SaveCanvas(NodeEditorInputInfo info)
        {
            if (info.inputEvent.type == EventType.Layout)
                return;
            NodeEditorState state = info.editorState;
            if(!string.IsNullOrEmpty(state.canvas.savePath))
            {
                NodeEditorSaveManager.SaveNodeCanvas(state.canvas.savePath, ref state.canvas, true);
                EditorWindow.focusedWindow.ShowNotification(new GUIContent("Canvas Saved!"));
                NodeEditorCallbacks.IssueOnSaveCanvas(state.canvas);
            }
            else
                EditorWindow.focusedWindow.ShowNotification(new GUIContent("No save location found. Use 'Save As' [Ctrl+Alt+S]"));
            info.inputEvent.Use();
        }
        [Hotkey(KeyCode.S, EventModifiers.Control | EventModifiers.Alt)]
        private static void SaveCanvasAs(NodeEditorInputInfo info)
        {
            if (info.inputEvent.type == EventType.Layout)
                return;
            NodeEditorState state = info.editorState;
            string panelPath = NodeEditor.editorPath + "Resources/Saves/";
            if (state.canvas != null && !string.IsNullOrEmpty(state.canvas.savePath))
                panelPath = state.canvas.savePath;
            string path = UnityEditor.EditorUtility.SaveFilePanelInProject("Save Node Canvas", "Node Canvas", "asset", "", panelPath);
            if (!string.IsNullOrEmpty(path))
                NodeEditorSaveManager.SaveNodeCanvas(path, ref state.canvas, true);
            info.inputEvent.Use();
        }
        [Hotkey(KeyCode.S, EventModifiers.Command)]
        private static void SaveCanvasMac(NodeEditorInputInfo info)
        {
            SaveCanvas(info);
        }
        [Hotkey(KeyCode.S, EventModifiers.Command | EventModifiers.Alt)]
        private static void SaveCanvasAsMac(NodeEditorInputInfo info)
        {
            SaveCanvasAs(info);
        }
        #endregion

        #endregion;
    }
}
