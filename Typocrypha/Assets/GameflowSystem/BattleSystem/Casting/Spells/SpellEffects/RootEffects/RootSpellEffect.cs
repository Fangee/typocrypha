using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameflow.GUIUtilities;

public abstract class RootSpellEffect : ReorderableSOList<RootSpellEffect>.ListItem
{
    public TargetData targetPattern;
    public abstract void castEffect(Battlefield field, ICaster caster, CastData data);
    protected float TargetGUIHeight { get { return lineHeight * 5; } }
    protected Rect targetGUI(Rect rect)
    {
        return new Rect(rect);
    }
}
