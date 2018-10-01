
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ATB2
{
    public partial class Enemy : ICaster
    {
        // Container for enemy data
        public EnemyData enemyData;

        // Visual Data
        public Image image; // enemy sprite
        public Animator animator; // animates sprite

        #region ICaster implementation
        private int _health; // health property field
        public int Health
        {
            get
            {
                return _health; // returns field
            }
            set
            {
                _health = Mathf.Clamp(value, 0, Stats.maxHP); // allocates field
            }
        }
        private int _stagger; // stagger property field
        public int Stagger
        {
            get
            {
                return _stagger;
            }
            set
            {
                _stagger = Mathf.Clamp(value, 0, Stats.maxStagger);
            }
        }
        public string Name { get { return enemyData.name; } }
        public Vector3 WorldPos { get { return transform.position; } set { transform.position = value; } }
        private Battlefield.Position _position;
        public Battlefield.Position FieldPos { get { return _position; } set { _position = value >= 0 ? value : _position; } }
        private Battlefield.Position _target;
        public Battlefield.Position TargetPos { get { return _target; } set { _target = value >= 0 ? value : _target; } }
        public CasterStats Stats { get { return enemyData.stats; } }
        public bool Stunned { get { return isCurrentState("Stunned"); } }
        public bool Dead { get { return Health <= 0; } }
        public ICasterType CasterType { get { return ICasterType.ENEMY; } } 
        #endregion

        // UI Objects 
        public GameObject healthBar;
        public GameObject staggerBar;
        public GameObject chargeBar;

        // AI State
        public EnemyAI AI;

        // Setup function
        public override void Setup()
        {
            // Attaches sprite to SpriteRenderer
            image = enemyData.image;

            // Sets AI
            AI = EnemyAI.GetAIFromString(enemyData.AIType, enemyData.AIParameters);

            // Sets properies
            Health = Stats.maxHP;
            Stagger = Stats.maxStagger;
            charge = 0;
        }
    }
}
