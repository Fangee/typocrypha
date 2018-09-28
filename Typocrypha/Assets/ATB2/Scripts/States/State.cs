using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB2
{
    // A state for an actor in the ATB system
    public class State : StateMachineBehaviour
    {
        public List<string> onEnterEvent; // Events called when the state exits
        public List<string> onExitEvent; // Events called when the state exits
        public List<KeyCode> keyTriggers; // Keys pressed to send events while in this state
        public List<string> onKeyEvent; // Events called when associated keys are pressed (parallel w/ keyTriggers)

        Actor actor; // Actor this state is a part of
        bool continueEventSent; // Has the continue event been sent this iteration? (should only be sent once)
        float savedProgress; // Save progress to resume if state is interrupted

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //Debug.Log("Enter:" + stateInfo.shortNameHash);
            // First time set actor field
            if (actor == null) actor = animator.GetComponent<Actor>();

            // Reset continue flag
            continueEventSent = false;
            float tmpProgress = savedProgress;
            savedProgress = 0f;

            // Start state from old progress if saved
            if (tmpProgress != 0f)
            {
                animator.Play(stateInfo.shortNameHash, 0, tmpProgress);
            }
            // Otherwise, start as normal
            else
            {
                // Send enter events
                if (onEnterEvent.Count != 0)
                    foreach (string enterEvent in onEnterEvent)
                        actor.sendEvent(enterEvent);
            }
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Send continue event if reached end of state (so that multiple states can't exit at once)
            if (!continueEventSent && !stateInfo.loop && stateInfo.normalizedTime >= 1f)
            {
                actor.sendEvent("stateContinue");
                continueEventSent = true;
            }

            if (!continueEventSent)
            {
                // Save progress if flag is set
                if (animator.GetBool("SaveProgress"))
                    savedProgress = stateInfo.normalizedTime;
                // Check for key events
                for (int i = 0; i < keyTriggers.Count; i++)
                {
                    if (Input.GetKeyDown(keyTriggers[i]))
                        actor.sendEvent(onKeyEvent[i]);
                }
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //Debug.Log("Exit:" + stateInfo.shortNameHash);

            // Send exit events
            if (onExitEvent.Count != 0)
                foreach (string exitEvent in onExitEvent)
                    actor.sendEvent(exitEvent);
        }
    }
}

