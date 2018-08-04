using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeEditorFramework;

namespace TypocryphaGameflow
{
    [Node(false, "Event/Set Background", new System.Type[] { typeof(GameflowCanvas) })]
    public class setBackgroundNode : BaseNodeIO
    {
        #region Editor
        public enum BgType
        {
            Sprite,
            Prefab,
        }
        public override string Title { get { return "Set Background"; } }
        public override Vector2 MinSize { get { return new Vector2(150, 40); } }

        private const string Id = "setBackground";
        public override string GetID { get { return Id; } }

        public BgType bgType;
        public Sprite bgSprite;
        public GameObject bgPrefab;

        protected override void OnCreate()
        {
            bgType = BgType.Sprite;
        }

        public override void NodeGUI()
        {
            GUILayout.BeginVertical("box");
            bgType = (BgType)EditorGUILayout.EnumPopup(bgType);
            if (bgType == BgType.Sprite)
                bgSprite = EditorGUILayout.ObjectField(bgSprite, typeof(Sprite), false) as Sprite;
            else
                bgPrefab = EditorGUILayout.ObjectField(bgPrefab, typeof(GameObject), false) as GameObject;
            GUILayout.EndVertical();
        }
        #endregion

        #region Game
        public override ProcessFlag process()
        {
            if (bgType == BgType.Sprite)
                BackgroundEffects.main.setSpriteBG(bgSprite);
            else
                BackgroundEffects.main.setPrefabBG(bgPrefab);
            return ProcessFlag.Continue;
        }
        #endregion
    }
}
