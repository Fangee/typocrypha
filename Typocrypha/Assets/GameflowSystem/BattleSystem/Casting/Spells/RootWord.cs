using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "RootWord", menuName = "Spell Word/Root")]
public class RootWord : SpellWord {

    public int power;                   //Spell's intensity (not necessarily just damage)
    public float cooldown;              //Spell's base cooldown
    public int hitPercentage;           //Spell's base hit chance (1 = 1%)
    public int critPercentage;          //Spell's base crit chance (1 = 1%)
    public SpellTagSet tags;
    public virtual void doGUILayout()
    {
        EditorGUILayout.LabelField("poopy b hole");
    }
}

#region GUI
// CharacterData inspector (read-only)
[CustomEditor(typeof(RootWord))]
public class RootWordInspector : Editor
{

    public override void OnInspectorGUI()
    {
        RootWord data = target as RootWord;

        GUILayout.Label("Root Word: " + data.name);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        data.doGUILayout();

        if (GUI.changed)
            EditorUtility.SetDirty(data);
    }
}
#endregion
