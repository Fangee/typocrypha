using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB
{
    // Interprets messages from states (enemy events)
    public partial class ATBMessageInterpreter: MonoBehaviour
    {
        // Initializes enemy events
        static void initEnemy()
        {
            eventMap.Add(new ATBMessageSig(MessageType.exit, StateType.enemyPreCast), enemyEnterCast);
            eventMap.Add(new ATBMessageSig(MessageType.exit, StateType.enemyAfterCast), exitCast);
            eventMap.Add(new ATBMessageSig(MessageType.exit, StateType.enemyStunned), enemyExitStun);
            eventMap.Add(new ATBMessageSig(MessageType.stun, StateType.enemyCharge), enemyStunCharge);
            eventMap.Add(new ATBMessageSig(MessageType.stun, StateType.enemyPreCast), enemyStunPreCast);
        }

        // Pause all other actors, begin enemy's cast state
        static void enemyEnterCast(ATBMessage message, BattleField battleField)
        {
            enterSolo(message.actor, battleField);
            exitDefault(message, battleField);
        }

        // Send enemy to the stunned state from charge
        static void enemyStunCharge(ATBMessage message, BattleField battleField)
        {
            // Save previous charge process
            message.actor.stateMachine.SetBool("SaveProgress", true);
            // Go to stunned state
            message.actor.stateMachine.Play("Stunned");
        }

        // Send enemy to the stunned state from precast
        static void enemyStunPreCast(ATBMessage message, BattleField battleField)
        {
            // Go to stunned state
            message.actor.stateMachine.Play("Stunned");
        }

        // Exit from stunned state, restore stagger
        static void enemyExitStun(ATBMessage message, BattleField battleField)
        {
            Enemy enemy = (Enemy)message.actor;
            enemy.stagger = enemy.enemyData.maxStagger;
            exitDefault(message, battleField);
        }
    }
}

