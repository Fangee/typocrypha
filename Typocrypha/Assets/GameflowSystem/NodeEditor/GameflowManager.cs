using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;

namespace TypocryphaGameflow
{
    public class GameflowManager : MonoBehaviour
    {    
        public static GameflowManager main = null; // Global static ref
        public GameflowCanvas gameflow;
        BaseNode currNode;

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
            if (currNode.process() == BaseNode.ProcessFlag.Continue)
                next();
            //Else currNode.process() == BaseNode.ProcessFlag.Wait (wait for callback from BattleManager.cs or DialogueManager.cs to continue)
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
