using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ATB2
{
    public partial class Player : ICaster
    {
        public BattleKeyboard status_manager;
        public CasterStats stats;

        //Constructors
        #region ICaster implementation
        private int _health = 200; // health property field
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
        public int Stagger
        {
            get
            {
                return 0;
            }
            set
            {
                return;
            }
        }
        public string Name { get { return PlayerDataManager.main.PlayerName; } }
        public Vector3 WorldPos { get { return transform.position; } set { transform.position = value; } }
        public Battlefield.Position FieldPos { get; set; }
        public Battlefield.Position TargetPos { get; set; }
        public CasterStats Stats { get { return stats; } }
        public bool Stunned { get { return false; } }
        public bool Dead { get { return Health <= 0; } }
        public ICasterType CasterType { get { return ICasterType.PLAYER; } }
        private CasterTagDictionary _tags = new CasterTagDictionary();
        public CasterTagDictionary Tags { get { return _tags; } }
        #endregion

        //Restores player's HP and Shields to Maximum
        public void restoreToFull()
        {
            Health = Stats.maxHP;
        }
    }
}
