using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeEditorFramework;

namespace TypocryphaGameflow
{
    public class GameflowManager : MonoBehaviour
    {    
        public static GameflowManager main = null; // Global static ref
        public GameflowCanvas gameflow;
        public GameManagers managers = new GameManagers();
        BaseNode currNode;

        void Awake()
        {
            if (main == null) main = this;
            else Destroy(this);
        }
        private void Start()
        {
            //managers.battleManager.setEnabled(false);
            managers.dialogManager.setEnabled(false);
            StartCoroutine(waitForLoad());
        }

        // waits for files to load
        IEnumerator waitForLoad()
        {
            yield return new WaitUntil(() => AudioPlayer.main.ready);
            AudioPlayer.main.stopAll();
            yield return new WaitUntil(() => EnemyDatabase.main.is_loaded);
            yield return new WaitUntil(() => AllyDatabase.main.is_loaded);
            yield return new WaitUntil(() => AnimationPlayer.main.ready);
            Debug.Log("done loading assets");
            gameflowStart();
        }
        //Find the starting node of the canvas and start the game
        public void gameflowStart()
        {
            Debug.Log("gameflowStart");
            currNode = gameflow.getStartNode();
            next();
        }
        // Go to next item
        public void next()
        {
            //TODO: Item skipping/disabling
            currNode = currNode.next();
            if (currNode.process(managers) == BaseNode.ProcessFlag.Continue)
                next();
            //Else currNode.process() == BaseNode.ProcessFlag.Wait (wait for callback from BattleManager.cs or DialogueManager.cs to continue)
        }
    }
    [System.Serializable]
    public class GameManagers
    {
        public DialogManager dialogManager;
        public CharacterManager characterManager;
        //public BattleManagerS battleManager;
        public WaveManager waveManager;
        public BackgroundEffects backgroundManager;
        public TextEvents textEventManager;
        public PlayerDataManager playerDataManager;
    }
    [CustomPropertyDrawer(typeof(GameManagers))]
    public class GameManagersDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            Rect UIrect = new Rect(position.position, new Vector2(position.width, EditorGUIUtility.singleLineHeight));

            EditorGUI.LabelField(UIrect, new GUIContent("Managers"), new GUIStyle(GUIStyle.none) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
            UIrect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(UIrect, property.FindPropertyRelative("dialogManager"));
            UIrect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(UIrect, property.FindPropertyRelative("characterManager"));
            UIrect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(UIrect, property.FindPropertyRelative("waveManager"));
            UIrect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(UIrect, property.FindPropertyRelative("backgroundManager"));
            UIrect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(UIrect, property.FindPropertyRelative("textEventManager"));
            UIrect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(UIrect, property.FindPropertyRelative("playerDataManager"));

            EditorGUI.EndProperty();
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 7;
        }
    }
}
