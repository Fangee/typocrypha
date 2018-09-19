using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB
{
    public class Player : Actor
    {
        // Player attempts to cast a spell
        public void playerCast()
        {
            ATBManager.sendATBMessage(MessageType.cast, currStateType, 
                stateMachine.GetCurrentAnimatorStateInfo(0).fullPathHash, this);
        }
    }
}

