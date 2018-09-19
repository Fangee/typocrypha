using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ATB
{
    // A state for an actor in ATB system; Should be attached to all states in Animator
    public class State : StateMachineBehaviour
    {
        public StateType type; // Type of state this is (see 'ATBManager.cs' for enum)
        bool eventSent = false; // Has this state sent an event yet? (states should only send at most 1 event per state visit)
        float progress; // Old progress saved if interrupted

        //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Reset state
            eventSent = false;
            //animator.ResetTrigger("Continue");
            // Set current state on actor
            animator.GetComponent<Actor>().currStateType = type;
            // Restore old progress if interrupted previously
            float tmpProgress = progress;
            progress = 0f;
            if (animator.GetBool("SaveProgress") && (tmpProgress > 0f && tmpProgress < 1f))
            {
                animator.SetBool("SaveProgress", false);
                animator.Play(stateInfo.fullPathHash, -1, tmpProgress);
            }
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            progress = stateInfo.normalizedTime;
            // Only send at most 1 event per state
            if (!eventSent)
            {
                // Send onExit message when state reaches end; State doesn't actually exit until manager allows it with the "Continue" trigger
                if (!stateInfo.loop && stateInfo.normalizedTime >= 1f)
                {
                    ATBManager.sendATBMessage(MessageType.exit, type, stateInfo.fullPathHash, animator.GetComponent<Actor>()); // Send message
                    eventSent = true;
                }
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //
        //}

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}
    }
}

