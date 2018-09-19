using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB
{
    // Types of states actors can be in
    public enum StateType
    {
        enemyStart, // Enemy when spawned
        playerIdle, // Player's default state
        enemyCharge, // Enemy is charging a spell
        enemyPreCast, // Enemy finished charging, about to cast
        enemyCast, // Enemy is casting
        enemyStunned, // Enemy is stunned
        playerStart, // Player when spawned
        playerCast, // Player is casting
        any, // Any state
        allyStart, // Ally at the beginning of the battle
        allyCharge, // Ally charging MP
        allyCast, // Ally is using a skill
        playerAfterCast, // Section after player casts; can activate ally spells
        playerBeforeCast, // Section before player casts
        enemyBeforeCast, // Section before enemy casts; can activate ally spells
        enemyAfterCast, // Section after enemy casts
        allyStunned, // Ally is stunned
        allyBeforeCast, // Section before ally casts
        allyAfterCast, // Section after ally casts
        allyMenu, // Ally spell menu is up, and player can use ally skill
        SIZE // Total number of states
    }

    // Types of messages sent from states
    public enum MessageType
    {
        exit, // Sent from an exiting of a state
        stun, // Sent to a state to interrupt it into the stunned state
        cast, // An actor (player or actor) is actively casting a spell
        castMenu // An actor is opening the cast menu (ally casting)
    }

    // ATBMessage signifier, used for identifying different messages
    public struct ATBMessageSig
    {
        public MessageType messageType; // Type of this message
        public StateType stateType; // State actor is currently in
        public ATBMessageSig(MessageType _messageType, StateType _stateType)
        {
            messageType = _messageType;
            stateType = _stateType;
        }
    }

    // Messages sent from states to the manager
    public struct ATBMessage
    {
        public ATBMessageSig signifier; // Determines type/context of message
        public int hashID; // Hashed name of the state actor is currently in
        public Actor actor; // Actor that sent this message

        public ATBMessage(MessageType _messageType, StateType _stateType, int _hashID, Actor _actor)
        {
            signifier = new ATBMessageSig(_messageType, _stateType);
            hashID = _hashID;
            actor = _actor;
        }
    }

    // Interprets messages from states (main structures)
    public partial class ATBMessageInterpreter
    {
        // Event delegate for interpreted messages
        public delegate void ATBEvent(ATBMessage message, BattleField battleField);
        // Map from messages to events
        public static Dictionary<ATBMessageSig, ATBEvent> eventMap = new Dictionary<ATBMessageSig, ATBEvent>();
        // Stack for managing when actors have solo activity (casting)
        public static Stack<Actor> soloStack = new Stack<Actor>();

        // Initializes dictionary
        public static void init()
        {
            // Initialize message/event mappings
            initBase();
            initPlayer();
            initAlly();
            initEnemy();
            // If message has state type signifier of "any", add copies for all state types to map
            // If key is already in dictionary, ignores it (allows for overwriting for specific cases)
            List<KeyValuePair<ATBMessageSig, ATBEvent>> newItems = new List<KeyValuePair<ATBMessageSig, ATBEvent>>();
            foreach (var kvp in eventMap)
            {
                if (kvp.Key.stateType == StateType.any)
                {
                    for (int i = 0; i < (int)StateType.SIZE; i++)
                    {
                        ATBMessageSig sig = new ATBMessageSig(kvp.Key.messageType, (StateType)i);
                        if (!eventMap.ContainsKey(sig))
                            newItems.Add(new KeyValuePair<ATBMessageSig, ATBEvent>(sig, kvp.Value));
                    }
                }
            }
            foreach (var kvp in newItems)
                eventMap.Add(kvp.Key, kvp.Value);
        }

        // Give actor solo activity (the only actor unpaused)
        public static void enterSolo(Actor soloActor, BattleField battleField)
        {
            // Pause all other actors and block all other enemies
            foreach (Actor actor in battleField.allActors)
            {
                if (actor != soloActor)
                {
                    actor.pause = true;
                    if (actor.GetType() == typeof(global::Enemy))
                        actor.blocked = true;
                }
            }
            // Unpause and unblock self
            soloActor.pause = false;
            soloActor.blocked = false;
            // Push onto the solo stack
            soloStack.Push(soloActor);
        }

        // End actor's solo activity of actor at top of the stack
        public static void exitSolo(BattleField battleField)
        {
            soloStack.Pop();
            // If stack is now empty, unpause and unblock all actors
            if (soloStack.Count == 0)
            {
                foreach (Actor actor in battleField.allActors)
                {
                    actor.pause = false;
                    actor.blocked = false;
                }
            }
            // Otherwise, give next actor in stack solo status
            else
            {
                foreach (Actor actor in battleField.allActors)
                {
                    if (actor != soloStack.Peek())
                    {
                        actor.pause = true;
                        if (actor.GetType() == typeof(global::Enemy))
                            actor.blocked = true;
                    }
                    else
                    {
                        actor.pause = false;
                        actor.blocked = false;
                    }
                }
            }

        }
    }
}

