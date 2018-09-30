using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ATB2
{
    // An entity with state in the ATB system; can send messages to the state manager
    public abstract class Actor : MonoBehaviour
    {
        public Animator stateMachine; // State machine for this actor
        public int currStateHash // State name hash for current state
        {
            get
            {
                return stateMachine.GetCurrentAnimatorStateInfo(0).shortNameHash;
            }
        }
        public bool pause // Is this actor paused or not?
        {
            get
            {
                return stateMachine.speed == 0f;
            }
            set
            {
                onSetPause(value);
            }
        }

        // Called when the pause property is set
        protected void onSetPause(bool value)
        {
            if (value) stateMachine.speed = 0f;
            else       stateMachine.speed = 1f;
        }

        // Call to do initial setup on actor
        public abstract void Setup();

        // Sends message from this actor to state manager
        public void sendEvent(string stateEvent)
        {
            StateManager.sendEvent(stateEvent, this, currStateHash);
        }

        // Checks if the current state name matches given string
        public bool isCurrentState(string stateName)
        {
            return currStateHash == Animator.StringToHash(stateName);
        }
    }
}

