using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CastParser {
    public enum ParseResults
    {
        Valid,
        TooManyWords,
        InvalidWords,
        EmptySpell
    }
    //Returns Valid if the string array represents a valid spell that exists in the given spell dictionary
    public static ParseResults parse(string[] spellwords, SpellDictionary dict, out SpellData s)
    {
        s = new SpellData(null);
        if (spellwords.Length > 3)
            return ParseResults.TooManyWords;
        else if (spellwords.Length == 3)
            return SpellData.isValid(s.setData(spellwords[1], spellwords[0], spellwords[2]), dict) ? ParseResults.Valid : ParseResults.InvalidWords;
        else if (spellwords.Length == 2)
        {
            if (SpellData.isValid(s.setData(spellwords[1], spellwords[0]), dict))
                return ParseResults.Valid;
            return SpellData.isValid(s.setData(spellwords[0], null, spellwords[1]), dict) ? ParseResults.Valid : ParseResults.InvalidWords;
        }
        else if (spellwords.Length == 1)
            return SpellData.isValid(s.setData(spellwords[0]), dict) ? ParseResults.Valid : ParseResults.InvalidWords;
        else
            return ParseResults.EmptySpell;
    }
}
