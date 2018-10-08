using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

[CreateAssetMenu(fileName = "CasterTag", menuName = "Caster Tag")]
public class CasterTag : ScriptableObject
{
    public CasterStats statMods;
    public ReactionDict tagReactions;
    public TagSet subTags;
    public AbilitySet abilities;

    public void dataGUILayout()
    {
        EditorGUI.indentLevel++;
        statMods.doModifierDataGUILayout();
        EditorGUI.indentLevel--;
    }




    [System.Serializable] public class ReactionDict : SerializableDictionary<SpellTag, Elements.vsElement> { }
    [System.Serializable] public class TagSet : SerializableSet<CasterTag> { }
    [System.Serializable] public class TagDict : SerializableDictionary<string, CasterTag> { }
    [System.Serializable] public class AbilitySet : SerializableSet<CasterAbility> { }
}

#region Caster Tag Inspector GUI
// CharacterData inspector (read-only)
[CustomEditor(typeof(CasterTag))]
public class CasterTagInspector : Editor
{

    public override void OnInspectorGUI()
    {
        CasterTag tag = target as CasterTag;

        GUILayout.Label("Caster Tag: " + tag.name);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Stat Modifiers", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter});
        tag.statMods.doGUILayout(true);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        //data.tags.doGUILayout("Tags");
    }
}
#endregion

public class SpellTag
{
    string keyword;

}

public class CasterAbility
{

}

