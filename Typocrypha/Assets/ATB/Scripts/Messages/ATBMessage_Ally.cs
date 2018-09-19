using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB
{
    // Interprets messages from states (ally events)
    public partial class ATBMessageInterpreter : MonoBehaviour
    {
        // Initializes ally events
        static void initAlly()
        {
            eventMap.Add(new ATBMessageSig(MessageType.castMenu, StateType.allyCharge), allyMenu);
            eventMap.Add(new ATBMessageSig(MessageType.cast, StateType.allyMenu), allyEnterCast);
            eventMap.Add(new ATBMessageSig(MessageType.exit, StateType.allyBeforeCast), allyCast);
            eventMap.Add(new ATBMessageSig(MessageType.exit, StateType.allyAfterCast), exitCast);
            eventMap.Add(new ATBMessageSig(MessageType.exit, StateType.allyMenu), exitCast);
        }

        // Check cast conditions, open cast menu
        static void allyMenu(ATBMessage message, BattleField battleField)
        {
            Ally ally = (Ally)message.actor;
            Player player = battleField.player;
            // Check MP cost
            if (ally.currMP < ally.castCost) return;
            bool cast = false;
            // Check activation timing: right before player casts
            if (player.currStateType == StateType.playerBeforeCast)
            {
                player.pause = true; // Pause player
                cast = true;
            }
            // Check activation timing: right after player casts
            if (player.currStateType == StateType.playerAfterCast)
            {
                exitSolo(battleField);
                player.stateMachine.Play("Idle");
                cast = true;
            }
            foreach (Enemy enemy in battleField.enemies)
            {
                // Check activation timing: right before an enemy casts
                if (enemy.currStateType == StateType.enemyBeforeCast)
                {
                    cast = true;
                }
                // Check activation timing: right after an enemy casts
                if (enemy.currStateType == StateType.enemyAfterCast)
                {
                    exitSolo(battleField);
                    enemy.stateMachine.Play("Charge");
                    cast = true;
                }
            }
            foreach (Ally otherAlly in battleField.allies)
            {
                if (otherAlly == ally) continue;
                // Check activation timing: right after other ally casts
                if (otherAlly.currStateType == StateType.allyAfterCast)
                {
                    exitSolo(battleField);
                    otherAlly.stateMachine.Play("Charge");
                    cast = true;
                }
            }
            // If cast conditions met, activate spell effects
            if (cast)
            {
                // Go to menu state 
                ally.stateMachine.Play("Menu");
                enterSolo(ally, battleField);
            }
        }

        // Start ally's cast sequence
        static void allyEnterCast(ATBMessage message, BattleField battleField)
        {
            message.actor.stateMachine.Play("BeforeCast");
        }

        // Play ally spell cast effects; apply effects to targets
        static void allyCast(ATBMessage message, BattleField battleField)
        {
            Ally ally = (Ally)message.actor;
            // Reduce mana (COULD ALSO PUT THIS IN BEFORECAST STATE)
            ally.currMP -= ally.castCost;
            // PUT EFFECTS HERE
            exitDefault(message, battleField);
        }
    }
}
