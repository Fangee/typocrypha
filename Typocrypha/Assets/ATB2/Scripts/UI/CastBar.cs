using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ATB2
{
    // Manages cast bar where player types in spells
    public class CastBar : MonoBehaviour
    {
        // Input field for cast bar
        public InputField input;

        #region Static Solo Fields and Properties
        //The castBar to enable after soloing ends
        static CastBar _mainBar = null;
        //public access to the main bar (for toggling focus for non-caster solo actors)
        public static CastBar MainBar { get { return _mainBar; } }
        // Stack for showing only 1 castbar at once
        static readonly Stack<CastBar> soloStack = new Stack<CastBar>();
        //List to track all CastBars for soloing
        static readonly List<CastBar> allCastBars = new List<CastBar>();
        #endregion

        // Puts bar in/out focus for typing
        public bool focus
        {
            get
            {
                return input.interactable;
            }
            set
            {
                input.interactable = true;
                if (value)
                {
                    input.ActivateInputField();
                    input.Select();
                }
                input.interactable = value;
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
            allCastBars.Add(this);
            if (gameObject.name == "PlayerCastBar")
                _mainBar = this;
        }

        private void OnDestroy()
        {
            allCastBars.Remove(this);
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
            /*
            if (Input.GetKeyDown(KeyCode.Backspace))
                AudioPlayer.main.playSFX("sfx_backspace");
            else if (Input.inputString.Length > 0)
                AudioPlayer.main.playSFX("sfx_type_key");
            */
        }

        // Shows only this cast bar
        public void solo()
        {
            foreach (CastBar bar in allCastBars)
                bar.hidden = true;
            this.hidden = false;
        }

        // Enters solo mode for this cast bar
        public static void enterSolo(CastBar soloBar)
        {
            soloBar.solo();
            soloStack.Push(soloBar);
        }

        // Exits solo mode for this cast bar (must be the last cast bar pushed)
        public static void exitSolo(CastBar soloBar)
        {
            if (soloBar != soloStack.Pop())
                Debug.LogError("StateManager: Solo Stack Mismatch");
            // If stack is empty, hide all except for player
            if (soloStack.Count == 0) 
            {
                foreach (CastBar bar in allCastBars)
                    bar.hidden = !(bar.focus = (bar == _mainBar));                 
            }
            // Otherwise, solo the next in the stack
            else
            {
                soloStack.Peek().solo();
            }
        }
    }
}

