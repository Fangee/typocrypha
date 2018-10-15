using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Gameflow.MathUtils;

[System.Serializable]
public class CasterStats
{
    public static readonly IntRange statRange = new IntRange(-10, 10);

    #region Resource Maxes
    public int maxHP;
    public int maxStagger;
    #endregion

    #region Stats
    [SerializeField] private float staggerTime;
    public float StaggerTime { get { return staggerTime; } set { staggerTime = value > 0 ? value : 0; } }
    [SerializeField] private int atk;
    public int Atk { get { return atk; } set { atk = statRange.clamp(value); } }
    [SerializeField] private int def;
    public int Def { get { return def; } set { def = statRange.clamp(value); } }
    [SerializeField] private int spd;
    public int Spd { get { return spd; } set { spd = statRange.clamp(value); } }
    [SerializeField] private int acc;
    public int Acc { get { return acc; } set { acc = statRange.clamp(value); } }
    [SerializeField] private int evade;
    public int Evade { get { return evade; } set { evade = statRange.clamp(value); } }
    #endregion

    #region GUI
    public void doGUILayout(bool isMod = false)
    {
        if (!isMod)
        {
            maxHP = EditorGUILayout.IntField(new GUIContent("Max HP", "TODO: tooltip"), maxHP);
            maxStagger = EditorGUILayout.IntField(new GUIContent("Max Stagger", "TODO: tooltip"), maxStagger);
            StaggerTime = EditorGUILayout.FloatField(new GUIContent("Stagger Time", "TODO: tooltip"), staggerTime);
        }
        atk = EditorGUILayout.IntSlider(new GUIContent("Atk", "TODO: tooltip"), atk, statRange.min, statRange.max);
        def = EditorGUILayout.IntSlider(new GUIContent("Def", "TODO: tooltip"), def, statRange.min, statRange.max);
        spd = EditorGUILayout.IntSlider(new GUIContent("Speed", "TODO: tooltip"), spd, statRange.min, statRange.max);
        acc = EditorGUILayout.IntSlider(new GUIContent("Acc", "TODO: tooltip"), acc, statRange.min, statRange.max);
        evade = EditorGUILayout.IntSlider(new GUIContent("Evade", "TODO: tooltip"), evade, statRange.min, statRange.max);
    }
    public void doModifierDataGUILayout()
    {
        string data = string.Empty;
        if (atk != 0)
            data += ("Atk " + (atk < 0 ? "" : "+") + atk + ", ");
        if (def != 0)
            data += ("Def " + (def < 0 ? "" : "+") + def + ", ");
        if (spd != 0)
            data += ("Spd " + (spd < 0 ? "" : "+") + spd + ", ");
        if (acc != 0)
            data += ("Acc " + (acc < 0 ? "" : "+") + acc + ", ");
        if (Evade != 0)
            data += ("Evade " + (evade < 0 ? "" : "+") + evade);
        if (!string.IsNullOrEmpty(data))
            EditorGUILayout.LabelField(data, new GUIStyle(GUI.skin.label) { wordWrap = true });
    }
    #endregion
}
