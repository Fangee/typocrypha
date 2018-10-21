using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//A class containing the required data to cast a spell (with defined keyword composition)
//Also contains associated methods like ToString()
[System.Serializable]
public class SpellData : IEnumerable<SpellWord>
{
    public List<SpellWord> words = new List<SpellWord>();

    //Returns a string representation of the spell (Display mode, with "-" delimiters and all caps)
    public override string ToString()
    {
        string result = string.Empty;
        foreach(SpellWord word in words)
            result += word.ToString() + '-';
        return result.TrimEnd('-').ToUpper();
    }
    //Returns the casting time of the spell (OBSOLETE)
    public float getCastingTime(float speed)
    {
        throw new System.Exception();
        //float time = dict.getRoot(root).cooldown;
        //float baseTime = time;
        //if (!string.IsNullOrEmpty(element))
        //{
        //    ElementMod e = dict.getElementMod(element);
        //    time += e.cooldownMod + (baseTime * e.cooldownModM);
        //}
        //if (!string.IsNullOrEmpty(style))
        //{
        //    StyleMod s = dict.getStyleMod(style);
        //    time += s.cooldownMod + (baseTime * s.cooldownModM);
        //    if (!string.IsNullOrEmpty(element))
        //        time -= baseTime;
        //}
        //return time / speed;
    }
    //Returns if the spelldata represents a valid spell
    public bool isValid()
    {
        throw new System.NotImplementedException();
    }

    #region IEnumarable Implementation
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return words.GetEnumerator();
    }

    IEnumerator<SpellWord> IEnumerable<SpellWord>.GetEnumerator()
    {
        return words.GetEnumerator();
    }
    #endregion
}