using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB2
{
    public partial class Ally : InputCaster
    {
        // UI Objects
        public GameObject healthUI;
        public GameObject manaUI;

        // Properties
        float _mana; // Current amount of time (seconds) spent charging current spell
        public float mana
        {
            get
            {
                return _mana;
            }
            set
            {
                _mana = value;
                manaUI.GetComponent<ShadowBar>().curr = _mana / maxMana;
            }
        }
        public float maxMana; // TESTING: max mana
        public float manaRate; // TESTING: rate at which mana is charged (per sec)
        public float manaCost; // TESTING: cost of spell

        Coroutine manaCRObj; // Coroutine that charges mana

        // Start charging mana
        public void startMana()
        {
            manaUI.GetComponent<ShadowBar>().reset();
            _mana = 0f;
            manaCRObj = StartCoroutine(manaCR());
        }

        // Incrementally charges mana
        IEnumerator manaCR()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                // Cap off mana
                if (mana >= maxMana)
                {
                    mana = maxMana;
                }
                // Charge while in charge state
                else
                {
                    if (!pause && isCurrentState("Charge"))
                        mana += manaRate * Time.fixedDeltaTime;
                }
            }
        }

        void Start()
        {
            Setup();
        }

        public override void Setup()
        {
            castBar.hidden = true;
            castBar.focus = false;
        }

        // Called when ally trigger is called
        public void menu()
        {
            sendEvent("allyMenu");
        }

        // Called when ally cast is entered
        public void cast()
        {
            sendEvent("allyStartCast");
        }
    }
}

