using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GUIUtilities;

public abstract class RootSpellEffect : ReorderableSOList<RootSpellEffect>.ListItem
{
    public TargetData targetPattern = new TargetData();
    public abstract void castEffect(Battlefield field, ICaster caster, CastData data);
}
