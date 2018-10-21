using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CustomDamageFormula : ScriptableObject {
    public abstract void doGUILayout();
    public abstract int calcDamage(Battlefield field, ICaster caster, CastResults data);
}
