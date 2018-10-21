using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AttackEffect : RootSpellEffect
{

    public override float Height
    {
        get
        {
            init();
            return lineHeight + targetPattern.GUIHeight + animationData.Height;
        }
    }

    public Damage.FormulaType damageFormula = Damage.FormulaType.Default;
    public CustomDamageFormula customFormula = null;

    public override void castEffect(Battlefield field, ICaster caster, CastResults data)
    {
        if (damageFormula == Damage.FormulaType.Custom)
        {
            customFormula.calcDamage(field, caster, data);
        }
        else
            Damage.standardFormula[damageFormula](field, caster, data);
    }

    public override void doGUI(Rect rect)
    {
        init();
        Rect UIRect = new Rect(rect) { height = EditorGUIUtility.singleLineHeight };
        GUI.Label(UIRect, new GUIContent("Attack", "TODO: tooltip"), new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold});
        UIRect.y += lineHeight;
        animationData.doList(UIRect);
        UIRect.y += animationData.Height;
        UIRect = targetPattern.doGUI(UIRect);
    }
}
