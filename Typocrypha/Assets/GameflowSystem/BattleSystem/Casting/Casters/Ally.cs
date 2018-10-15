using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB2
{

    public partial class Ally : ICaster
    {
        public AllyData data;

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
        public string Name { get { return data.name; } }
        public Vector3 WorldPos { get { return transform.position; } set { transform.position = value; } }
        public Battlefield.Position FieldPos { get; set; }
        public Battlefield.Position TargetPos { get; set; }
        public CasterStats Stats { get { return data.stats; } }
        public bool Stunned { get { return isCurrentState("Stunned"); } }
        public bool Dead { get { return Health <= 0; } }
        public ICasterType CasterType { get { return ICasterType.ALLY; } }
        public CasterTagDictionary Tags { get { return data.tags; } }
        #endregion

        // UI Objects (TODO)

        // AI State (TODO)

        // Setup from data (TODO)
        public void initStats()
        {
            // Sets properies
            Health = Stats.maxHP;
            Stagger = Stats.maxStagger;
        }
    }

}
