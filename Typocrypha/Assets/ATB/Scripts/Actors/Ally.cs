using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB
{
    public class Ally : Actor
    {
        public string allyName; // Name of this type of enemy
        public float maxMP; // Max possible MP
        public float MPRechargeRate; // Rate (per second) of MP recharge
        public float castCost; // MP cost of spell
        public RectTransform mpBar; // MP bar transform (for showing progress)
        float _currMP; // Current MP
        public float currMP
        {
            get
            {
                return _currMP;
            }
            set
            {
                _currMP = value;
                mpBar.localScale = new Vector3(_currMP/maxMP, mpBar.localScale.y, 1f);
            }
        }
        public KeyCode allyKey; // Key pressed to activate this ally

        void Start()
        {
            Setup();
        }

        void Update()
        {
            if (Input.GetKeyDown(allyKey))
                castMenu();
        }

        public override void Setup()
        {
            currMP = 0f;
            StartCoroutine(chargeMP());
        }

        // Charges MP (ticks based on fixed update loop)
        IEnumerator chargeMP()
        {
            yield return new WaitForSeconds(1f); // TESTING: start up time
            float step = MPRechargeRate * Time.fixedDeltaTime; // Amount updated per frame
            for (;;)
            {
                if (currMP + step > maxMP)
                    currMP = maxMP;
                else if (!pause && currStateType == StateType.allyCharge && currMP < maxMP)
                    currMP += step;     
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
        public void cast()
        {
            ATBManager.sendATBMessage(MessageType.cast, currStateType,
                stateMachine.GetCurrentAnimatorStateInfo(0).fullPathHash, this);
        }
    }
}

