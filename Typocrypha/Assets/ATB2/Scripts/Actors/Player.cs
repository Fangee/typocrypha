using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB2
{
    public partial class Player : InputCaster
    {
        void Start()
        {
            Setup();
        }

        public override void Setup()
        {
            castBar.hidden = false;
            castBar.focus = true;
        }

        // Called when player enters a spell into the cast bar
        public void cast()
        {
            sendEvent("playerStartCast");
        }

    }
}

