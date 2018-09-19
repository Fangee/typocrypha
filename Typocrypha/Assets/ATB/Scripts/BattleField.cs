using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB
{
    // Represents battle state
    public class BattleField : MonoBehaviour
    {
        public Player player;
        public Ally[] allies;
        public ATB.Enemy[] enemies;
        [HideInInspector] public List<Actor> allActors;

        void Start()
        {
            allActors = new List<Actor>();
            if (player != null)
                allActors.Add(player);
            foreach (Ally ally in allies)
                allActors.Add(ally);
            foreach (ATB.Enemy enemy in enemies)
                allActors.Add(enemy);
        }
    }
}

