using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TypocryphaGameflow;

namespace TypocryphaGameflow
{
    // Manages creation/deletion of characters in scene
    // Keeps track of current characters in scene, and dispatches commands to them
    public class CharacterManager : MonoBehaviour
    {
        Dictionary<string, Character> characters; // Characters display in current scene indexed by aliases (many to one)
        public Transform character_holder; // Parent transform for all character objects
        public GameObject character_prefab; // Prefab for character display

        void Awake()
        {
            characters = new Dictionary<string, Character>();
        }

        // Create a new character and put them into the scene
        public void addCharacter(CharacterData data, Vector2 pos, string pose, string expression)
        {
            GameObject chr_obj = Instantiate(character_prefab, character_holder);
            Character chr = chr_obj.GetComponent<Character>();
            chr.pose = pose;
            chr.expression = expression;
            foreach (string alias in data.aliases)
            {
                characters.Add(alias, chr);
            }
            
        }

        // Remove a character from the scene
        public void removeCharacter(string alias)
        {

        }
    }
}
