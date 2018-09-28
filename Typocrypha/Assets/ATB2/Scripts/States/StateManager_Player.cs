using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB2
{
    // Events sent from the player
    public partial class StateManager : MonoBehaviour
    {
        // Starts player's cast sequence 
        // Sent when player enters a spell into the cast bar
        public void playerStartCast(StateEventArgs args)
        {
            Player player = (Player)args.actor;
            enterSolo(player); // Enter solo, pausing all other actors
            CastBar.enterSolo(player.castBar);
            player.castBar.focus = false;
            args.actor.stateMachine.Play("BeforeCast");
        }

        // Apply player's cast effects
        // Sent when player enters Cast state
        public void playerCast(StateEventArgs args)
        {
            Debug.Log(args.actor.gameObject.name + " has cast a spell");
        }

        // End player's cast sequence
        // Sent when player exits AfterCast state
        public void playerEndCast(StateEventArgs args)
        {
            Player player = (Player)args.actor;
            exitSolo(player);
            CastBar.exitSolo(player.castBar);
            player.castBar.clear();
        }
    }
}
