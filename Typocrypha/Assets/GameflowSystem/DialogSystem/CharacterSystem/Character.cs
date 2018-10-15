using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameflow;

namespace Gameflow
{
    // Visual representation of a character in a dialogue scene
    public class Character : MonoBehaviour
    {
        public static Color dimColor = new Color(0.25f, 0.25f, 0.25f, 1);
        public static Color highlightColor = new Color(1f, 1f, 1f, 1f);

        public CharacterData data; // Relevant data on character (sprites, etc)
        public SpriteRenderer body; // Body sprite
        public SpriteRenderer face; // Face sprite
        public string pose // Set pose
        {
            set
            {
                body.sprite = data.poses[value];
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
                    body.color = highlightColor;
                    face.color = highlightColor;
                }
                else
                {
                    body.color = dimColor;
                    face.color = dimColor;
                }
            }
        }

        void Start()
        {
            highlight = false;
        }

        // Teleports character instantly to new location
        public void teleport(Vector2 pos)
        {
            transform.position = pos;
        }

    }
}
