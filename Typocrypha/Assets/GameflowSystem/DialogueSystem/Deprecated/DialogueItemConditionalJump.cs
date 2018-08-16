using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class DialogueItemConditionalJump : DialogueItemJumpBase {
//    public string conditionKey;
//    public string[] conditionCases;
//    public Dialogue[] targets;
//    public override Dialogue evaluateTarget()
//    {
//        string val = PlayerDialogueInfo.main.getInfo(conditionKey).ToLower();
//        for(int i = 0; i < conditionCases.Length; ++i)
//            if (val == conditionCases[i].ToLower())
//                return targets[i];
//        Debug.LogWarning("Conditional Jump: " + conditionKey + " has not been set, defaulting to targets[0]");
//        return targets[0];
//    }
//}
