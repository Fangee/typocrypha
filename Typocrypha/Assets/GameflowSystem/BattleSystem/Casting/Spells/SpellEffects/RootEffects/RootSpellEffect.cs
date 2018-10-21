using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GUIUtilities;

public abstract class RootSpellEffect : ReorderableSOList<RootSpellEffect>.ListItem
{
    [SerializeField] protected List<SpellAnimationData> _animationData = new List<SpellAnimationData>();
    public ReorderableList<SpellAnimationData> animationData = null;
    public TargetData targetPattern = new TargetData();
    public abstract void castEffect(Battlefield field, ICaster caster, CastResults data);
    protected void init()
    {
        if(animationData == null)
            animationData = new ReorderableList<SpellAnimationData>(_animationData, true, true, new GUIContent("Animation Data"));
    }
}
