using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB2
{
    // Manages state events; also contains all events that can be sent (organized into partial classes)
    public partial class StateManager : MonoBehaviour
    {
        // Overall battle state
        public Battlefield battleField;
        // Events sent
        static List<StateEventObj> eventQueue = new List<StateEventObj>(); 
        // Stack for managing when actors have solo activity (casting)
        public static Stack<Actor> soloStack = new Stack<Actor>();

        // Check queue for new messages
        void Update()
        {
            processQueue();
        }

        // Process queue (FIFO: one event per update frame)
        void processQueue()
        {
            if (eventQueue.Count == 0) return;
            StateEventObj obj = eventQueue[0];
            this.SendMessage(obj.stateEvent, obj.args);
            eventQueue.Remove(obj);
        }

        // Send an event to the manager to put in the queue
        public static void sendEvent(string stateEvent, StateEventArgs args)
        {
            eventQueue.Add(new StateEventObj(stateEvent, args));
        }

        // Send an event to the manager to put in the queue
        public static void sendEvent(string stateEvent, Actor actor, int hashID)
        {
            //Debug.Log("Send event:" + actor.gameObject.name + ":" + stateEvent);
            sendEvent(stateEvent, new StateEventArgs(actor, hashID));
        }

        // Set the pause value of all actors
        void setPauseAll(bool value)
        {
            foreach (Actor actor in battleField.Actors)
                actor.pause = value;
        }

        // Enter solo mode for this actor
        void enterSolo(Actor soloActor)
        {
            if (soloStack.Count == 0)
            {
                setPauseAll(true);
                CastBar.MainBar.focus = false;
            }
            else
                soloStack.Peek().pause = true;
            soloActor.pause = false;
            soloStack.Push(soloActor);
        }

        // Exit solo mode for this actor (should be at top of stack)
        void exitSolo(Actor soloActor)
        {
            if (soloActor != soloStack.Pop())
                Debug.LogError("StateManager: Solo Stack Mismatch");
            // If stack is now empty, unpause all actors
            if (soloStack.Count == 0)
            {
                setPauseAll(false);
                CastBar.MainBar.focus = true;
                foreach (Actor actor in battleField.actorsToAdd)
                    actor.isCast = false;
            }
            // Otherwise, give solo to next in stack
            else
            {
                soloActor.pause = true;
                soloStack.Peek().pause = false;
            }
        }
    }
}

