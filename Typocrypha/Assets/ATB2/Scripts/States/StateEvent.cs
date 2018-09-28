using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB2
{
    // Event functions called to modify the game state
    public delegate void StateEvent(StateEventArgs args);

    // Arguments to a state event
    public class StateEventArgs
    {
        public Actor actor; // Actor this event came from 
        public int hashID; // Hash of state state machine was currently in

        public StateEventArgs(Actor _actor, int _hashID)
        {
            actor = _actor;
            hashID = _hashID;
        }

        public override string ToString()
        {
            return actor.gameObject.name + ":" + hashID;
        }
    }

    // Container for event functions and arguments
    public class StateEventObj
    {
        public string stateEvent; // Name of function to call (in StateManager)
        public StateEventArgs args; // Arguments to function

        public StateEventObj(string _stateEvent, Actor _actor, int _hashID)
        {
            stateEvent = _stateEvent;
            args = new StateEventArgs(_actor, _hashID);
        }

        public StateEventObj(string _stateEvent, StateEventArgs _args)
        {
            stateEvent = _stateEvent;
            args = _args;
        }
    }
}

