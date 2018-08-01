using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;

namespace TypocryphaGameflow
{
    public class GameflowManager : MonoBehaviour
    {    
        public static GameflowManager main = null; // Global static ref
        public GameObject player_ui; // the Typocrypha UI 
                                     //public GameObject screenframe_vn;
                                     //public GameObject screenframe_battle;
        public GameflowCanvas gameflow;

        BaseGameflowNode currNode;

        void Awake()
        {
            if (main == null) main = this;
            else Destroy(this);
        }
        private void Start()
        {
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

        public void gameflowStart()
        {
            Debug.Log("gameflowStart");
            currNode = gameflow.getStartNode();
            next();
        }

        // Go to next item
        public void next()
        {
            currNode = currNode.next();
            //TODO: Item skipping/disabling
            if (currNode is DialogManagerNode)
            {
                Debug.Log("starting dialogue: " + currNode.name);
                player_ui.SetActive(false);
                BattleManagerS.main.setEnabled(false);
                DialogueManager.main.startDialogue(currNode as DialogManagerNode);
            }
            else if (currNode is BattleManagerNode)
            {
                Debug.Log("starting battle: " + currNode.name);
                player_ui.SetActive(true);
                DialogueManager.main.setEnabled(false);
                //BattleManagerS.main.startBattle(currNode as BattleManagerNode);
            }
            else if (currNode is BaseEventNode)
            {
                (currNode as BaseEventNode).processEvent();
                next();
            }
            else
                next();
        }
        // Jump to item
        public void jump(GameObject targetGameFlowItem, bool goToNext = true)
        {
            Debug.Log("Gameflow: Jumping to " + targetGameFlowItem.name);
            //curr_gameflow = targetGameFlowItem.transform.parent.GetComponent<Gameflow>();
            //curr_gameflow.curr_item = targetGameFlowItem.transform.GetSiblingIndex() - 1;
            //if (goToNext) next();
        }
    }
}
