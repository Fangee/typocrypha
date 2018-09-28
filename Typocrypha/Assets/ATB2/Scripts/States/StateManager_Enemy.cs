using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB2
{
    // Events sent from enemy actors
    public partial class StateManager : MonoBehaviour
    {
        // Start charging next spell
        // Called when enemy enters charge state
        public void enemyStartCharge(StateEventArgs args)
        {
            ((Enemy)args.actor).startCharge();
        }

        // Go into enemy's PreCast state
        // Sent when enemy finishes charging spell
        public void enemyPreCast(StateEventArgs args)
        {
            args.actor.stateMachine.Play("PreCast");
        }

        // Start enemy's cast sequence
        // Sent when enemy enters BeforeCast state
        public void enemyStartCast(StateEventArgs args)
        {
            enterSolo(args.actor); // Enter solo, pausing all other actors
            battleField.player.castBar.focus = false;
        }

        // Apply enemy's cast effects
        // Sent when enemy enters Cast state
        public void enemyCast(StateEventArgs args)
        {
            Debug.Log(args.actor.gameObject.name + " has cast a spell");
        }

        // End enemy's cast sequence
        // Sent when enemy exits AfterCast state
        public void enemyEndCast(StateEventArgs args)
        {
            exitSolo(args.actor); // Exit solo mode
            battleField.player.castBar.focus = true;
        }
    }
}


