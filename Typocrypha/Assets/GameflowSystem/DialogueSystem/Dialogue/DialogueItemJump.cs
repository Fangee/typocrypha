using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Represents a jump to a different dialogue section
public class DialogueItemJump : DialogueItemJumpBase {
	public Dialogue target; // Dialogue to jump to
    public override Dialogue evaluateTarget() {
        return target;
    }
}
