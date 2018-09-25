﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ATB
{
    // Manages cast bar where player types in spells
    public class CastBar : MonoBehaviour
    {
        // Input field for cast bar
        public InputField input;
        // Stack for showing only 1 castbar at once
        static Stack<CastBar> soloStack = new Stack<CastBar>();
        // Puts bar in/out focus for typing
        bool _focus;
        public bool focus
        {
            get
            {
                return _focus;
            }
            set
            {
                _focus = value;
                input.interactable = true;
                if (value)
                {
                    input.ActivateInputField();
                    input.Select();
                }
                else
                {
                    Debug.Log("Defocus");
                    input.DeactivateInputField();
                }
                input.interactable = value;
                input.readOnly = !value;
            }
        }
        // Shows/hides castbar
        public bool hidden
        {
            set
            {
                if (value)
                {
                    transform.Translate(new Vector3(1024,1024,0));
                }
                else
                {
                    transform.position = pos;
                }
            }
        }
        // Original position
        Vector3 pos;

        void Awake()
        {
            pos = transform.position;
            input.onValueChanged.AddListener(keySFX);
        }

        // Clears cast bar
        public void clear()
        {
            input.text = "";
            GetComponentInChildren<Text>().text = "";
        }

        // Called on value changed to make typing effects
        public void keySFX(string value)
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
                AudioPlayer.main.playSFX("sfx_backspace");
            else if (Input.inputString.Length > 0)
                AudioPlayer.main.playSFX("sfx_type_key");
        }

        // Shows only this cast bar
        public void solo()
        {
            CastBar[] bars = FindObjectsOfType<CastBar>();
            foreach (CastBar bar in bars)
                bar.hidden = true;
            this.hidden = false;
        }

        // Enters solo mode for this cast bar
        public void enterSolo()
        {
            this.solo();
            soloStack.Push(this);
        }

        // Exits solo mode for this cast bar (must be the last cast bar pushed)
        public void exitSolo()
        {
            soloStack.Pop();
            // If stack is empty, hide all except for player
            if (soloStack.Count == 0) 
            {
                CastBar[] bars = FindObjectsOfType<CastBar>();
                foreach (CastBar bar in bars)
                    if (bar.gameObject.name == "PlayerCastBar") bar.hidden = false;
                    else                                        bar.hidden = true;
                    
            }
            // Otherwise, solo the next in the stack
            else
            {
                soloStack.Peek().solo();
            }
        }
    }
}

