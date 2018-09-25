using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB
{
    // Class for actors that can cast actively (i.e. player and allies)
    public abstract class Caster : Actor
    {
        public CastBar castBar; // This caster's cast bar

        // Called when pause is set
        public new void OnSetPause(bool value)
        {
            base.OnSetPause(value);
        }
    }
}

