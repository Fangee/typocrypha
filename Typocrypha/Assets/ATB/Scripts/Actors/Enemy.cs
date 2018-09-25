using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ATB
{
    // Enemies that the player fights against
    public class Enemy : Actor
    {
        // Container for enemy data
        public EnemyData enemyData;

        // UI Objects 
        public Image spriteImage; // enemy sprite
        public GameObject healthUI; // health bar
        public GameObject chargeUI; // charge bar
        public GameObject staggerUI; // stagger shield
        public Text enemyName; // name of enemy
        public Text spellName; // name of next spell to be cast

        // AI State
        public EnemyAI AI;

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
                healthUI.GetComponent<ShadowBar>().curr = (float)_health / (float)enemyData.maxHP;
            }
        }

        public float chargeTime // charge time property field; total charge time for current spell
        {
            get
            {
                return 1f / stateMachine.GetFloat("ChargeTime");
            }
            set
            {
                stateMachine.SetFloat("ChargeTime", 1f / value);
            }
        }

        public float charge // charge property field; current charging progress (normalized)
        {
            get
            {
                return chargeUI.transform.Find("Amount").GetComponent<RectTransform>().localScale.x;
            }
            set // can only be set if current state is charge state
            {
                if (currStateType == StateType.enemyCharge)
                {
                    stateMachine.Play("Charge", 0, value);
                }
            }
        }

        private int _stagger; // stagger property field
        public int stagger
        {
            get
            {
                return _stagger;
            }
            set
            {
                staggerUI.GetComponentInChildren<Text>().text = value.ToString();
                if (value <= 0) // stun enemy if stagger at 0
                {
                    staggerUI.GetComponentInChildren<Animator>().Play("Stunned");
                    ATBManager.sendATBMessage(MessageType.stun, currStateType,
                        stateMachine.GetCurrentAnimatorStateInfo(0).fullPathHash, this);
                }
                else if (stagger == 0) // unstun enemy if recovering from stun
                {
                    staggerUI.GetComponentInChildren<Animator>().Play("Unstunned");
                }
                _stagger = value;
            }
        }

        void Start()
        {
            Setup(); // TESTING
        }

        // Setup function
        public override void Setup()
        {
            // Sets properties from enemy data
            health = enemyData.maxHP;
            stagger = enemyData.maxStagger;
            enemyName.text = enemyData.name;
            spriteImage.sprite = enemyData.sprite;

            // Sets AI
            AI = EnemyAI.GetAIFromString(enemyData.AIType, enemyData.AIParameters);

            // TESTING
            chargeTime = 5f;
        }

        // Called when pause is set
        public new void OnSetPause(bool value)
        {
            base.OnSetPause(value);
            blocked = value;
        }

        // TESTING: Stagger this enemy by 1
        public void staggerEnemy()
        {
            stagger--;
        }

        // TESTING: Damage enemy by 5
        public void damageEnemy()
        {
            health -= 30;
        }
    }
}

