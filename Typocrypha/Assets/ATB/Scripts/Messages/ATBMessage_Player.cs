using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB
{
    // Interprets messages from states (player events)
    public partial class ATBMessageInterpreter : MonoBehaviour
    {
        // Initializes player events
        static void initPlayer()
        {
            eventMap.Add(new ATBMessageSig(MessageType.cast, StateType.playerIdle), playerEnterCast);
            eventMap.Add(new ATBMessageSig(MessageType.exit, StateType.playerBeforeCast), playerCast);
            eventMap.Add(new ATBMessageSig(MessageType.exit, StateType.playerAfterCast), playerExitCast);
        }

        // Check cast conditions, and start player's cast sequence
        static void playerEnterCast(ATBMessage message, BattleField battleField)
        {
            enterSolo(message.actor, battleField);
            battleField.castBar.input.interactable = false;
            // Go to cast pause state (beginning of cast sequence)
            message.actor.stateMachine.Play("BeforeCast");
        }

        // Play player spell cast effects; apply effects to targets
        static void playerCast(ATBMessage message, BattleField battleField)
        {
            Debug.Log("Player casts:" + battleField.castBar.input.text);
            // PUT EFFECTS HERE
            exitDefault(message, battleField);
        }

        // Exit out of player cast sequence
        static void playerExitCast(ATBMessage message, BattleField battleField)
        {
            battleField.castBar.input.interactable = true;
            battleField.castBar.input.text = "";
            exitCast(message, battleField);
        }
    }
}

