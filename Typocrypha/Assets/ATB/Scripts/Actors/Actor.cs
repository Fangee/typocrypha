using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB
{
    // Baseclass for entities with state in the ATB system
    public class Actor : MonoBehaviour
    {
        public Animator stateMachine; // ATB state machine: Uses Unity animation system
        [HideInInspector]public StateType currStateType; // Current state type state machine is in
        public bool pause // Control pausing this actor (setting animator speed to 0 or 1)
        {
            get { return stateMachine.speed == 0f; }
            set
            {
                if (value) stateMachine.speed = 0f;
                else       stateMachine.speed = 1f;
            }
        }
        [HideInInspector]public bool blocked; // Can this actor send messages to the ATB system?

    }
}
