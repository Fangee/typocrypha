using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TypocryphaGameflow
{
    // Container for data about specific characters
    public class CharacterData : ScriptableObject
    {
        public HashSet<string> aliases; // Different aliases/names for this character
        public UnityEngine.AudioClip talk_sfx; // Talking sound effect
        public Dictionary<string, Sprite> bodies; // Poses
        public Dictionary<string, Sprite> expressions; // Facial expressions
        public Dictionary<string, Sprite> codecs; // Codec sprites
        public Sprite chat_icon; // Chat mode sprite
    }
}

