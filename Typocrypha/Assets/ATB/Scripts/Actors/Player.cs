using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ATB
{
    // Player caster
    public class Player : Caster
    {
        // PLAYER DATA

        // UI Objects
        public GameObject healthUI; // health bar

        // Propertes
        private int _health; // current health property field
        public int health
        {
            get
            {
                return _health;
            }
            set
            {
                _health = value;
                healthUI.GetComponent<ShadowBar>().curr = (float)_health / 200f;
            }
        }

        void Start()
        {
            Setup(); // TESTING
        }

        // Setup function
        public override void Setup()
        {
            castBar.hidden = false;
            castBar.focus = true;
            health = 200;
        }

        // Called when pause is set
        public override void OnSetPause(bool value)
        {
            base.OnSetPause(value);
            blocked = value;
        }

        // Player attempts to cast a spell (the spell typed out in cast bar)
        public void playerCast()
        {
            ATBManager.sendATBMessage(MessageType.cast, currStateType, 
                stateMachine.GetCurrentAnimatorStateInfo(0).fullPathHash, this);
        }
    }
}

