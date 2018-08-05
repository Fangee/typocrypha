using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TypocryphaGameflow;

namespace TypocryphaGameflow
{
    // Visual representation of a character in a dialogue scene
    public class Character : MonoBehaviour
    {
        public CharacterData data; // Relevant data on character (sprites, etc)
        public SpriteRenderer body; // Body sprite
        public SpriteRenderer face; // Face sprite
        public string pose // Set pose
        {
            set
            {
                body.sprite = data.bodies[value];
            }
        }
        public string expression // Set facial expression
        {
            set
            {
                face.sprite = data.expressions[value];
            }
        }
        bool _highlight; // Is character highlighted?
        public bool highlight // Set/get highlight status
        {
            get
            {
                return _highlight;
            }
            set
            {
                _highlight = value;
                if (_highlight)
                {
                    // HIGHLIGHT
                }
                else
                {
                    // DIM
                }
            }
        }

        void Start()
        {
            highlight = false;
        }

    }
}
