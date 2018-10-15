using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellDictionary : MonoBehaviour
{
    //Dictionaries containing spells associated with keywords
    private Dictionary<string, Spell> rootWords = new Dictionary<string, Spell>();
    private Dictionary<string, ElementMod> elements = new Dictionary<string, ElementMod>();
    private Dictionary<string, StyleMod> styles = new Dictionary<string, StyleMod>();

    #region Building (TO BE DEPRECATED)
    //Loads the spell dictionary at the beginning of the game
    public void Start()
    {
        is_loaded = false;
        buildDicts(); // load gameflow file
        is_loaded = true;
        Debug.Log("Dict loaded");
    }

    public bool is_loaded; // is the spellDict done loading?

    TextAsset text_file; // original text asset
    public string file_name; // name of gameflow file
    char[] line_delim = { '\n' };
    char[] col_delim = { '\t' };

    // parses Dictionary file which should be a tab-delimited txt file (from excel)
    public void buildDicts()
    {
        text_file = Resources.Load<TextAsset>(file_name);
        string[] lines = text_file.text.Split(line_delim);
        string[] cols;
        int i = 1;
        //read in root keywords
        while (true)
        {
            cols = lines[i].Split(col_delim);
            int ind = 0;
            string key = cols[ind].Trim();
            if (key.CompareTo("END") == 0) //At end of root keywords
            {
                i += 2;//Skip example line
                break;
            }
            string type = cols[++ind].Trim();
            Spell s = Spell.createSpellFromType(type);
            s.name = key;
            s.type = type;
            s.description = cols[++ind].Trim().Replace("\"", string.Empty);
            s.animationID = cols[++ind].Trim();
            s.sfxID = cols[++ind].Trim();
            int.TryParse(cols[++ind].Trim(), out s.power);
            float.TryParse(cols[++ind].Trim(), out s.cooldown);
            int.TryParse(cols[++ind].Trim(), out s.hitPercentage);
            int.TryParse(cols[++ind].Trim(), out s.critPercentage);
            int.TryParse(cols[++ind].Trim(), out s.elementEffectMod);
            string pattern = cols[++ind].Trim();
            //if (pattern.Contains("A"))
            //{
            //    s.targetData = new TargetData(true);
            //    s.targetData.selfCenter = false;
            //    s.targetData.targeted = false;
            //}
            //else
            //{
            //    s.targetData = new TargetData(false);
            //    if (pattern.Contains("L"))
            //        s.targetData.enemyL = true;
            //    if (pattern.Contains("M"))
            //        s.targetData.enemyM = true;
            //    if (pattern.Contains("R"))
            //        s.targetData.enemyR = true;
            //    if (pattern.Contains("l"))
            //        s.targetData.allyL = true;
            //    if (pattern.Contains("m"))
            //        s.targetData.allyM = true;
            //    if (pattern.Contains("r"))
            //        s.targetData.allyR = true;
            //    if (pattern.Contains("S"))
            //        s.targetData.selfCenter = true;
            //    if (pattern.Contains("T"))
            //        s.targetData.targeted = true;
            //}
            string buff = cols[++ind].Trim();
            if (!buff.Contains("N"))
            {
                s.buff = new BuffData();
                int level;
                if (type == "buff")
                    level = s.power;
                else
                    level = 1;
                if (buff.Contains("A"))
                    s.buff.attackMod = -1 * level;
                else if (buff.Contains("a"))
                    s.buff.attackMod = level;
                if (buff.Contains("D"))
                    s.buff.defenseMod = -1 * level;
                else if (buff.Contains("d"))
                    s.buff.defenseMod = level;
                if (buff.Contains("S"))
                    s.buff.speedMod = -1 * level;
                else if (buff.Contains("s"))
                    s.buff.speedMod = level;
                if (buff.Contains("H"))
                    s.buff.accuracyMod = -1 * level;
                else if (buff.Contains("h"))
                    s.buff.accuracyMod = level;
                if (buff.Contains("E"))
                    s.buff.evasionMod = -1 * level;
                else if (buff.Contains("e"))
                    s.buff.evasionMod = level;
                if (buff.Contains("U"))
                    s.buff.vsElemMod[Elements.@null] = -1 * level;
                else if (buff.Contains("u"))
                    s.buff.vsElemMod[Elements.@null] = level;
                if (buff.Contains("F"))
                    s.buff.vsElemMod[Elements.fire] = -1 * level;
                else if (buff.Contains("f"))
                    s.buff.vsElemMod[Elements.fire] = level;
                if (buff.Contains("I"))
                    s.buff.vsElemMod[Elements.ice] = -1 * level;
                else if (buff.Contains("i"))
                    s.buff.vsElemMod[Elements.ice] = level;
                if (buff.Contains("B"))
                    s.buff.vsElemMod[Elements.volt] = -1 * level;
                else if (buff.Contains("b"))
                    s.buff.vsElemMod[Elements.volt] = level;
            }
            int.TryParse(cols[++ind].Trim(), out s.buffPercentage);
            switch (cols[++ind].Trim().ToLower())
            {
                case "no_mods":
                    s.modFlag = Spell.ModFlags.NO_MODIFICATION;
                    break;
                case "no_elements":
                    s.modFlag = Spell.ModFlags.NO_ELEMENT;
                    break;
                case "no_styles":
                    s.modFlag = Spell.ModFlags.NO_STYLE;
                    break;
                case "no_targeting":
                    s.modFlag = Spell.ModFlags.NO_TARGETING;
                    break;
                default:
                    s.modFlag = Spell.ModFlags.NORMAL;
                    break;
            }
            rootWords.Add(key, s);
            i++;
        }
        //read in element keywords
        while (true)
        {
            cols = lines[i].Split(col_delim);
            int ind = 0;
            string key = cols[ind].Trim();
            if (key.CompareTo("END") == 0)
            {
                i += 2;//Skip example line
                break;
            }
            ElementMod e = new ElementMod();
            e.name = key;
            e.element = Elements.fromString(cols[++ind].Trim());
            e.description = cols[++ind].Trim().Replace("\"", string.Empty);
            e.animationID = cols[++ind].Trim();
            e.sfxID = cols[++ind].Trim();
            float.TryParse(cols[++ind].Trim(), out e.cooldownMod);
            float.TryParse(cols[++ind].Trim(), out e.cooldownModM);
            elements.Add(key, e);
            i++;
        }
        //Read in casting style keywords
        while (true)
        {
            cols = lines[i].Split(col_delim);
            int ind = 0;
            string key = cols[ind].Trim();
            if (key.CompareTo("END") == 0)
            {
                return;
            }
            StyleMod s = new StyleMod();
            s.name = key;
            int.TryParse(cols[++ind].Trim(), out s.powerMod);
            s.description = cols[++ind].Trim().Replace("\"", string.Empty);
            s.animationID = cols[++ind].Trim();
            s.sfxID = cols[++ind].Trim();
            float.TryParse(cols[++ind].Trim(), out s.powerModM);
            float.TryParse(cols[++ind].Trim(), out s.cooldownMod);
            float.TryParse(cols[++ind].Trim(), out s.cooldownModM);
            int.TryParse(cols[++ind].Trim(), out s.accMod);
            float.TryParse(cols[++ind].Trim(), out s.accModM);
            int.TryParse(cols[++ind].Trim(), out s.critMod);
            float.TryParse(cols[++ind].Trim(), out s.critModM);
            int.TryParse(cols[++ind].Trim(), out s.statusEffectChanceMod);
            float.TryParse(cols[++ind].Trim(), out s.statusEffectChanceModM);
            string[] pattern = cols[++ind].Trim().Split(',');
            if (pattern[0].Contains("N"))
                s.isTarget = false;
            //else
            //    s.targets = TargetMod.createFromString(pattern[0].Trim('"'), pattern);
            styles.Add(key, s);
            i++;
        }
    }
    #endregion

    #region Dicitonary Management
    public bool containsRoot(string word)
    {
        return rootWords.ContainsKey(word);
    }
    public bool containsElement(string word)
    {
        return elements.ContainsKey(word);
    }
    public bool containsStyle(string word)
    {
        return styles.ContainsKey(word);
    }
    public Spell getRoot(SpellData data)
    {
        return rootWords[data.root];
    }
    public Spell getRoot(string word)
    {
        return rootWords[word];
    }
    public ElementMod getElementMod(SpellData data)
    {
        return elements[data.element];
    }
    public ElementMod getElementMod(string word)
    {
        return elements[word];
    }
    public StyleMod getStyleMod(SpellData data)
    {
        return styles[data.style];
    }
    public StyleMod getStyleMod(string word)
    {
        return styles[word];
    }
    #endregion

    //Get an array holding the animation data array for a given SpellData
    public string[] getAnimData(SpellData data)
    {
        string[] ret = { null, null, null };
        if (data.style != null)
            ret[0] = styles[data.style].animationID;
        if (data.root != null)
            ret[1] = rootWords[data.root].animationID;
        if (data.element != null)
            ret[2] = elements[data.element].animationID;
        return ret;
    }
    //Get an array holding the sfx data array for a given SpellData
    public string[] getSfxData(SpellData data)
    {
        string[] ret = { null, null, null };
        if (data.style != null)
            ret[0] = styles[data.style].sfxID;
        if (data.root != null)
            ret[1] = rootWords[data.root].sfxID;
        if (data.element != null)
            ret[2] = elements[data.element].sfxID;
        return ret;
    }
    //Return the targeting pattern
    public Pair<bool[], bool[]> getTargetPattern(SpellData data, Battlefield field, ICaster caster)
    {
        //if (!string.IsNullOrEmpty(data.style) || !styles[data.style].isTarget)
        //{
        //    return rootWords[data.root].targetData.toArrayPair(field, caster);
        //}
        //else
        //{
        //    TargetData t = new TargetData(false);
        //    t.copyFrom(rootWords[data.root].targetData);
        //    styles[data.style].targets.modify(t);
        //    return t.toArrayPair(field, caster);
        //}
        throw new System.Exception();
    }
}
//A class containing the required data to cast a spell (with defined keyword composition)
//Also contains associated methods like ToString()
[System.Serializable]
public class SpellData
{
    public string root;
    public string element;
    public string style;

    //Make a new spelldata instance with root keyword root, element keyword prefix, and style keyword suffix
    public SpellData(string root, string prefix = null, string suffix = null)
    {
        this.root = root;
        element = prefix;
        style = suffix;
    }
    //Set the spellData all at once
    public SpellData setData(string root, string prefix = null, string suffix = null)
    {
        this.root = root;
        element = prefix;
        style = suffix;
        return this;
    }
    //Returns a string representation of the spell (Display mode, with "-" delimiters and all caps)
    public override string ToString()
    {
        string result;
        if (!string.IsNullOrEmpty(element))
            result = element + "-" + root;
        else
            result = root;
        if (!string.IsNullOrEmpty(style))
            result += ("-" + style);
        return result.ToUpper();
    }
    //Returns a string representation of a component of a spell (used for displaying casts in spelleffects)
    public string getWord(int index)
    {
        if (index == 0)
            return style;
        if (index == 1)
            return root;
        if (index == 2)
            return element;
        throw new System.Exception("Not a valid spell index");
    }
    //Returns the casting time of the spell
    public float getCastingTime(SpellDictionary dict, float speed)
    {
        float time = dict.getRoot(root).cooldown;
        float baseTime = time;
        if (!string.IsNullOrEmpty(element))
        {
            ElementMod e = dict.getElementMod(element);
            time += e.cooldownMod + (baseTime * e.cooldownModM);
        }
        if (!string.IsNullOrEmpty(style))
        {
            StyleMod s = dict.getStyleMod(style);
            time += s.cooldownMod + (baseTime * s.cooldownModM);
            if (!string.IsNullOrEmpty(element))
                time -= baseTime;
        }
        return time / speed;
    }
    //Static isValid
    public static bool isValid(SpellData s, SpellDictionary d)
    {
        return s.isValid(d);
    }
    //Returns if the spelldata represents a valid spell
    public bool isValid(SpellDictionary dict)
    {
        return dict.containsRoot(root) 
                && string.IsNullOrEmpty(element) ? true : dict.containsElement(element) 
                && string.IsNullOrEmpty(style) ? true : dict.containsStyle(style);
    }
    //Returns a copy of this spellData
    public SpellData clone()
    {
        return new SpellData(root, element, style);
    }
}