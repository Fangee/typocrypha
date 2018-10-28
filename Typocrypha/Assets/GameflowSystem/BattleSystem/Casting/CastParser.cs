using System.Collections.Generic;
using RandomUtils;

public static class CastParser
{
    public static readonly Dictionary<string, SpellWord> words = build();
    private static Dictionary<string, SpellWord> build()
    {
        List<RootWord> rootWords = AssetUtils.GetAssetList<RootWord>("ScriptableObjects/SpellWords/Roots");
        List<ModifierWord> modifiers = AssetUtils.GetAssetList<ModifierWord>("ScriptableObjects/SpellWords/Modifiers");
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
        EmptySpell,
        TooManyWords,
        NoRoot,
        TooManyRoots,
        TypoFailure,
    }
    private enum TypoResult
    {
        CastFailure,
        DropoutWord,
        ReplaceWord,
    }
    private static readonly List<WeightedRandom.Value<TypoResult>> typoActionWeighting = new List<WeightedRandom.Value<TypoResult>>()
    {
        WeightedRandom.CreateValue(0.85f, TypoResult.CastFailure),
        WeightedRandom.CreateValue(0.10f, TypoResult.DropoutWord),
        WeightedRandom.CreateValue(0.05f, TypoResult.ReplaceWord)
    };

    private const int maxWords = 5;
    private const int maxRoots = 3;

    //Returns Valid if the string array represents a valid spell that exists in the given spell dictionary
    //Returns other ParseResults to indicate different failure conditions
    public static ParseResults parse(string[] spellwords, out SpellData s)
    {
        s = new SpellData();
        int roots = 0;
        foreach(string word in spellwords)
        {
            if (words.ContainsKey(word))
            {
                s.words.Add(words[word]);
                if (words[word].Type == SpellWord.WordType.Root)
                    ++roots;
            }
            else
            {
                #region Process Typo Results
                TypoResult result = typoActionWeighting.ChooseByRandom();
                switch (result)
                {
                    case TypoResult.CastFailure:
                        return ParseResults.TypoFailure;
                    case TypoResult.ReplaceWord:
                        SpellWord replacement = replaceTypo(word);
                        s.words.Add(replacement);
                        if (replacement.Type == SpellWord.WordType.Root)
                            ++roots;
                        break;
                    default:
                        continue;
                }
                #endregion
            }
        }

        #region Keyword Number Checks
        if (s.words.Count <= 0)
            return ParseResults.EmptySpell;
        if (s.words.Count > maxWords)
            return ParseResults.TooManyWords;
        #endregion

        #region Root Number Checks
        if (roots <= 0)
            return ParseResults.NoRoot;
        if (roots > maxRoots)
            return ParseResults.TooManyRoots;
        #endregion

        return ParseResults.Valid;
    }
    //Returns a replacement word for a typo keyword (TODO)
    private static SpellWord replaceTypo(string word)
    {
        //TODO: add actual replacement keyword option
        return words["splash"];
    }
}
