using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ATB
{
    public class Player : Actor
    {
        // PLAYER STATS?

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
            health = 200;
        }

        // Player attempts to cast a spell (the spell typed out in cast bar)
        public void playerCast()
        {
            ATBManager.sendATBMessage(MessageType.cast, currStateType, 
                stateMachine.GetCurrentAnimatorStateInfo(0).fullPathHash, this);
        }
    }
}

