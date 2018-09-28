using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB2
{
    // Base events used by various different actors and states
    public partial class StateManager : MonoBehaviour
    {
        // Print a debug message
        public void ping(StateEventArgs args)
        {
            Debug.Log("Ping:" + args);
        }

        // Allow the state to continue
        // Sent when a state is ready to exit
        public void stateContinue(StateEventArgs args)
        {
            args.actor.stateMachine.SetTrigger("Continue");
        }

        // Save the progress of the current state (will resume from that point next time)
        public void saveProgress(StateEventArgs args)
        {
            args.actor.stateMachine.SetBool("SaveProgress", true);
        }

        // Pause the actor who sent this event
        public void pause(StateEventArgs args)
        {
            args.actor.pause = true;
        }

        // Unpause the actor who sent this event
        public void unpause(StateEventArgs args)
        {
            args.actor.pause = false;
        }

        // Stun the actor who sent this event
        public void stun(StateEventArgs args)
        {
            args.actor.stateMachine.Play("Stunned");
        }
    }
}

