using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//Contains targeting data and associated targeting modification methods
[System.Serializable]
public class TargetData
{
    public enum Type
    {
        Targeted,
        Absolute,
        CasterCentered,
    }

    public BoolMatrix2D pattern = new BoolMatrix2D(2, 3);
    public Type type = Type.Targeted;

    public List<Battlefield.Position> target(Battlefield.Position casterPos, Battlefield.Position targetPos)
    {
        List<Battlefield.Position> ret = new List<Battlefield.Position>();

        #region Calculate Column Shift and Flipping
        bool flip = casterPos.Row == 0; //True if on enemy side, false if not
        int colShift = 0;
        if (type == Type.Targeted)
        {
            flip = flip ^ (casterPos.Row == targetPos.Row);
            colShift = 1 - (targetPos.Col);
        }
        else if (type == Type.CasterCentered)
            colShift = 1 - (targetPos.Col);
        #endregion

        #region Find Targets
        BoolMatrix2D targets = flip ? pattern.rotated180() as BoolMatrix2D : pattern;
        int startCol = System.Math.Max(0, colShift);
        int endCol = System.Math.Min(colShift, targets.Columns);
        for (int row = 0; row < targets.Rows; ++row)
            for (int col = startCol; col < endCol; ++col)
                if (targets[row, col])
                    ret.Add(new Battlefield.Position(row, col - colShift));
        #endregion

        return ret;
    }

    public float GUIHeight { get { return (EditorGUIUtility.singleLineHeight + 1) * 5; } }
    public Rect doGUI(Rect rect)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight + 1;
        Rect UIRect = new Rect(rect) { height = EditorGUIUtility.singleLineHeight };
        GUI.Label(UIRect, new GUIContent("Target Pattern", "TODO, tooltip"));
        UIRect.y += lineHeight;
        type = (Type)EditorGUI.EnumPopup(UIRect, type);
        UIRect.y += lineHeight;
        const float checkWidth = 50;
        const float labelWidth = 100;
        string topRowLabel = type == Type.Targeted ? "Target Row" : "Enemy Row";
        GUI.Label(new Rect(UIRect) { width = labelWidth }, new GUIContent(topRowLabel));
        Rect checkRect = new Rect(UIRect) { width = checkWidth, x = UIRect.x + labelWidth + 3 };
        pattern[0, 0] = EditorGUI.Toggle(checkRect, pattern[0, 0]);
        checkRect.x += checkWidth;
        pattern[0, 1] = EditorGUI.Toggle(checkRect, pattern[0, 1]);
        checkRect.x += checkWidth;
        pattern[0, 2] = EditorGUI.Toggle(checkRect, pattern[0, 2]);
        UIRect.y += lineHeight;
        string bottomRowLabel = type == Type.Targeted ? "Other Row" : "Ally Row";
        GUI.Label(new Rect(UIRect) { width = labelWidth }, new GUIContent(bottomRowLabel));
        checkRect = new Rect(UIRect) { width = checkWidth, x = UIRect.x + labelWidth + 3};
        pattern[1, 0] = EditorGUI.Toggle(checkRect, pattern[1, 0]);
        checkRect.x += checkWidth;
        pattern[1, 1] = EditorGUI.Toggle(checkRect, pattern[1, 1]);
        checkRect.x += checkWidth;
        pattern[1, 2] = EditorGUI.Toggle(checkRect, pattern[1, 2]);
        UIRect.y += lineHeight;
        return UIRect;

    }

    ////Modifies this by target data mod
    //public void modify(TargetData mod)
    //{
    //    bool targets_enemy = (enemyL || enemyM || enemyR);
    //    bool targets_ally = (allyL || allyM || allyR);
    //    bool mod_targets_enemy = (mod.enemyL || mod.enemyM || mod.enemyR);
    //    bool mod_targets_ally = (mod.allyL || mod.allyM || mod.allyR);
    //    if (targets_enemy && targets_ally && mod_targets_ally && mod_targets_enemy)
    //    {
    //        enemyL = mod.enemyL;
    //        enemyM = mod.enemyM;
    //        enemyR = mod.enemyR;
    //        allyL = mod.allyL;
    //        allyM = mod.allyM;
    //        allyR = mod.allyR;
    //        targeted = mod.targeted;
    //        selfCenter = mod.selfCenter;
    //    }
    //    else if (targets_enemy)
    //    {
    //        enemyL = mod.enemyL;
    //        enemyM = mod.enemyM;
    //        enemyR = mod.enemyR;
    //        targeted = mod.targeted;
    //    }
    //    else if (targets_ally)
    //    {
    //        allyL = mod.allyL;
    //        allyM = mod.allyM;
    //        allyR = mod.allyR;
    //        selfCenter = mod.selfCenter;
    //    }
    //}
    //public void copyFrom(TargetData toCopy)
    //{
    //    enemyL = toCopy.enemyL;
    //    enemyM = toCopy.enemyM;
    //    enemyR = toCopy.enemyR;
    //    targeted = toCopy.targeted;
    //    allyL = toCopy.allyL;
    //    allyM = toCopy.allyM;
    //    allyR = toCopy.allyR;
    //    selfCenter = toCopy.selfCenter;
    //}
}
