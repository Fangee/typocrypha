using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB
{
    // Interprets messages from states (base/general events)
    public partial class ATBMessageInterpreter : MonoBehaviour
    {
        // Initializes base events
        static void initBase()
        {
            eventMap.Add(new ATBMessageSig(MessageType.exit, StateType.any), exitDefault);
        }

        // Default exit message functionality
        static void exitDefault(ATBMessage message, BattleField battleField)
        {
            // Allow state to transition
            message.actor.stateMachine.SetTrigger("Continue");
        }

        // Unpause all other actors, exit out of cast sequence
        static void exitCast(ATBMessage message, BattleField battleField)
        {
            exitSolo(battleField);
            exitDefault(message, battleField);
        }
    }
}

