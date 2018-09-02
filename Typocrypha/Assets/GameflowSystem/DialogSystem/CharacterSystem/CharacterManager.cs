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
        Dictionary<string, CharacterData> _allCharacterData; // All character data indexed by aliases
        Dictionary<string, Character> characters; // Characters display in current scene indexed by aliases (many to one)
        public Transform characterHolder; // Parent transform for all character objects
        public GameObject characterPrefab; // Prefab for character display
        public List<CharacterData> allCharacterData; // List interface for inserting character data

        void Awake()
        {
            characters = new Dictionary<string, Character>();
            _allCharacterData = new Dictionary<string, CharacterData>();
            foreach (CharacterData data in allCharacterData)
            {
                foreach (string alias in data.aliases)
                {
                    _allCharacterData.Add(alias, data);
                }
            }
        }

        #region Character Control Node Handling
        // Applies a character control event to specified character
        public void characterControl(CharacterControlNode.EventData ev)
        {
            if (ev.GetType() == typeof(AddCharacter)) // Intercept character creation event
            {
                AddCharacter add = ev as AddCharacter;
                addCharacter(_allCharacterData[add.characterName], add.pos, add.startingPose, add.startingExpression);
            }
            else if (ev.GetType() == typeof(RemoveCharacter)) // Intercept character removal event
            {
                removeCharacter(ev.characterName);
            }
            else
            {
                ev.characterControl(characters[ev.characterName]);
            }
        }

        // Create a new character and put them into the scene
        void addCharacter(CharacterData data, Vector2 pos, string pose, string expression)
        {
            GameObject chr_obj = Instantiate(characterPrefab, characterHolder);
            Character chr = chr_obj.GetComponent<Character>();
            chr_obj.transform.position = pos;
            chr.data = data;
            chr.pose = pose;
            chr.expression = expression;
            foreach (string alias in data.aliases)
            {
                characters.Add(alias, chr);
            }
        }

        // Remove a character currently in the scene
        void removeCharacter(string alias)
        {
            // Destroy character object
            GameObject.Destroy(characters[alias].gameObject);
            // Destroy alias entries in characters dictionary
            NameSet names = _allCharacterData[alias].aliases;
            foreach (string del in names)
            {
                characters.Remove(del);
            }
        }

        //TODO: remove all characters
        #endregion

        #region Speaker Control
        // Highlight, set talk sfx for specified character (if in scene)
        public void speak(string characterName)
        {
            Character chara;
            if(characters.TryGetValue(DialogParser.main.substituteMacros(characterName), out chara))
            {
                soleHighlight(chara);
                AudioPlayer.main.setVoiceSFX(chara.data.talk_sfx);
            }
            //Else: have fallbacks?
            //Else: default
        }

        // Highlight the specified character and un-highlight all other characters
        void soleHighlight(Character chara)
        {
            chara.highlight = true;
            foreach (Character c in characters.Values)
            {
                if (c != chara)
                    c.highlight = false;
            }
        }
        #endregion
    }
}
