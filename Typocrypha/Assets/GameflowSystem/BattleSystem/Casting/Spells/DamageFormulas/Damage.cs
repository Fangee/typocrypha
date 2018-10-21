using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class Damage {
    public enum FormulaType
    {
        Default,
        Custom,
    }
    public delegate int Formula(Battlefield field, ICaster caster, CastResults data);
    public static Dictionary<FormulaType, Formula> standardFormula = new Dictionary<FormulaType, Formula>
    {
        {FormulaType.Default, null}
    };
}
