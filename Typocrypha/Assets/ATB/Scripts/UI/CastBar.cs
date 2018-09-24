using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ATB
{
    // Manages cast bar where player types in spells
    public class CastBar : MonoBehaviour
    {
        public InputField input;

        void Update()
        {
            input.Select(); // keep cast bar in focus (while enabled)
        }

        public void keySFX()
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
                AudioPlayer.main.playSFX("sfx_backspace");
            else if (Input.inputString.Length > 0)
                AudioPlayer.main.playSFX("sfx_type_key");
        }
        
    }
}

