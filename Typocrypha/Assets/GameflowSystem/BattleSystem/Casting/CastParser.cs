using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CastParser {

    public static Dictionary<string, SpellWord> words = build();

    private static Dictionary<string, SpellWord> build()
    {
        RootWord[] rootWords = Resources.LoadAll<RootWord>("ScriptableObjects/SpellWords/Roots");
        ModifierWord[] modifiers = Resources.LoadAll<ModifierWord>("ScriptableObjects/SpellWords/Modifiers");
        Dictionary<string, SpellWord> _words = new Dictionary<string, SpellWord>();
        foreach (RootWord w in rootWords)
            _words.Add(w.name.ToLower(), w);
        foreach (ModifierWord w in modifiers)
            _words.Add(w.name.ToLower(), w);
        return _words;
    }

    public enum ParseResults
    {
        Valid,
        TooManyWords,
        InvalidWords,
        EmptySpell,
        NoRoot,
    }
    //Returns Valid if the string array represents a valid spell that exists in the given spell dictionary
    public static ParseResults parse(string[] spellwords, out SpellData s)
    {
        s = new SpellData();
        if (spellwords.Length > 5)
            return ParseResults.TooManyWords;
        if (spellwords.Length <= 0)
            return ParseResults.EmptySpell;
        bool containsRoot = false;
        foreach(string word in spellwords)
        {
            if (words.ContainsKey(word))
            {
                s.words.Add(words[word]);
                if (words[word].Type == SpellWord.WordType.Root)
                    containsRoot = true;
            }
            else
                return ParseResults.InvalidWords;
        }
        if(containsRoot)
            return ParseResults.Valid;
        return ParseResults.NoRoot;
    }
}
