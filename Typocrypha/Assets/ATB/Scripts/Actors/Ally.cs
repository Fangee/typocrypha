using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB
{
    // Allys that help the player in battle with ally casting
    public class Ally : Caster
    {
        // ALLY DATA SCRIPTABLE OBJECT
        public float maxMP; // TESTING: Max possible MP
        public float MPRechargeRate; // TESTING: Rate (per second) of MP recharge
        public float castCost; // TESTING: MP cost of spell 

        // UI Objects
        public GameObject mpUI; // MP bar

        // Properties
        float _mp; // Current MP
        public float mp
        {
            get
            {
                return _mp;
            }
            set
            {
                _mp = value;
                mpUI.GetComponent<ShadowBar>().curr = value / maxMP;
            }
        }
        public KeyCode allyKey; // Key pressed to activate this ally

        void Start()
        {
            Setup();
        }

        void Update()
        {
            if (Input.GetKeyDown(allyKey)) castMenu();
        }

        // Setup function
        public override void Setup()
        {
            castBar.hidden = true;
            castBar.focus = false;
            mp = 0;
            StartCoroutine(chargeMPCR());
        }

        // Charges MP (ticks based on fixed update loop)
        IEnumerator chargeMPCR()
        {
            yield return new WaitForSeconds(1f); // TESTING: start up time
            float step = MPRechargeRate * Time.fixedDeltaTime; // Amount updated per frame
            for (;;)
            {
                if (mp + step > maxMP)
                    mp = maxMP;
                else if (!pause && currStateType == StateType.allyCharge && mp < maxMP)
                    mp += step;     
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }
        }

        // Stun this ally by sending stun message
        public void stun()
        {
            ATBManager.sendATBMessage(MessageType.stun, currStateType,
                stateMachine.GetCurrentAnimatorStateInfo(0).fullPathHash, this);
        }

        // Attempt to open the ally spell menu
        public void castMenu()
        {
            ATBManager.sendATBMessage(MessageType.castMenu, currStateType,
                stateMachine.GetCurrentAnimatorStateInfo(0).fullPathHash, this);
        }

        // Cast selected ally spell
        public void allyCast()
        {
            ATBManager.sendATBMessage(MessageType.cast, currStateType,
                stateMachine.GetCurrentAnimatorStateInfo(0).fullPathHash, this);
        }
    }
}

