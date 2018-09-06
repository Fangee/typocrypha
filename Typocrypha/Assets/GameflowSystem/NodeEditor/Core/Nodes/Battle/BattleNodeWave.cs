using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;
using System;

namespace TypocryphaGameflow
{
    [Node(false, "Battle/Wave", new System.Type[] { typeof(GameflowCanvas) })]
    public class BattleNodeWave : BaseNodeIO
    {
        public enum TransitionType
        {

        }

        public const string ID = "Battle Wave Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Battle Wave"; } }
        public override Vector2 MinSize { get { return new Vector2(250, 60); } }

        public string waveTitle;
        public AudioClip music;


        protected override void OnCreate()
        {
            base.OnCreate();
        }

        public override ScriptableObject[] GetScriptableObjects()
        {
            return base.GetScriptableObjects();
        }

        protected override void CopyScriptableObjects(Func<ScriptableObject, ScriptableObject> replaceSO)
        {
            base.CopyScriptableObjects(replaceSO);
        }

        public override void NodeGUI()
        {
            base.NodeGUI();
        }

        protected override void OnAddConnection(ConnectionPort port, ConnectionPort connection)
        {
            base.OnAddConnection(port, connection);
        }


        #region Game
        public override ProcessFlag process(GameManagers managers)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
