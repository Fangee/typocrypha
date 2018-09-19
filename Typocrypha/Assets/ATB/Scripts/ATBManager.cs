using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB
{
    // Options for inserting into message queue
    public enum QueuePos
    {
        front, // Insert to front of queue (next to be chosen in FIFO)
        rear // Insert to rear of queue (last to be chosen in FIFO)
    }

    // Manages interactions between states
    // Current conflict resolution strategy: only process one event per time interval (FIFO)
    public class ATBManager : MonoBehaviour
    {
        private static ATBManager instance = null;
        public BattleField battleField; // State of battle
        List<ATBMessage> messageQueue; // List of messages received to be processed

        void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(this);
            messageQueue = new List<ATBMessage>();
            ATBMessageInterpreter.init();
        }

        void Start()
        {
            Time.timeScale = 1f;
            StartCoroutine(checkMessages());
        }
        
        // Sends a message to the message queue for processing
        public static void sendATBMessage(MessageType messageType, StateType stateType, int hashID, Actor actor, QueuePos queuePos = QueuePos.rear)
        {
            switch (queuePos)
            {
                case QueuePos.rear:
                    instance.messageQueue.Add(new ATBMessage(messageType, stateType, hashID, actor));
                    break;
                case QueuePos.front:
                    instance.messageQueue.Insert(0, new ATBMessage(messageType, stateType, hashID, actor));
                    break;
            }
            
        }

        // Check message queue for new messages
        IEnumerator checkMessages()
        {
            for (;;)
            {
                processQueue();
                yield return null; // Wait one frame
            }
        }

        // Processes message queue; Then calls function to process next event if not blocked
        void processQueue()
        {
            if (messageQueue.Count == 0) return; // No Messages, so return
            ATBMessage currMessage = messageQueue[0]; // Get first message in the queue
            if (currMessage.actor.blocked) return; // Don't process event if actor is blocked
            processEvent(currMessage); // Execute event associated with message
            messageQueue.Remove(currMessage); // Remove message from queue
        }
        
        // Processes event sent by a message
        void processEvent(ATBMessage message)
        {
            // If message is mapped to an event, execute it
            if (ATBMessageInterpreter.eventMap.ContainsKey(message.signifier))
            {
                //Debug.Log("Process:" + message.actor.gameObject.name + ":" + message.signifier.messageType + ":" + message.signifier.stateType);
                ATBMessageInterpreter.eventMap[message.signifier](message, battleField);
            }
        }
    }
}

