using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Stores all the spell info and contains methods to parse and cast spells from player input
//Currently does not actually support player or enemy casting, but has parsing
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
            Spell s = createSpellFromType(type);
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
            if (pattern.Contains("A"))
            {
                s.targetData = new TargetData(true);
                s.targetData.selfCenter = false;
                s.targetData.targeted = false;
            }
            else
            {
                s.targetData = new TargetData(false);
                if (pattern.Contains("L"))
                    s.targetData.enemyL = true;
                if (pattern.Contains("M"))
                    s.targetData.enemyM = true;
                if (pattern.Contains("R"))
                    s.targetData.enemyR = true;
                if (pattern.Contains("l"))
                    s.targetData.allyL = true;
                if (pattern.Contains("m"))
                    s.targetData.allyM = true;
                if (pattern.Contains("r"))
                    s.targetData.allyR = true;
                if (pattern.Contains("S"))
                    s.targetData.selfCenter = true;
                if (pattern.Contains("T"))
                    s.targetData.targeted = true;
            }
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
            else
                s.targets = TargetMod.createFromString(pattern[0].Trim('"'), pattern);
            styles.Add(key, s);
            i++;
        }
    }
    #endregion

    #region Casting (TO BE RELOCATED)
    //Casts spell from NPC (enemy or ally)
    public List<CastData> cast(SpellData spell, ICaster[] targets, int selected, ICaster[] allies, int position, out List<Transform> noTargetPositions)
    {
        noTargetPositions = new List<Transform>();
        ICaster caster = allies[position];
        Spell s = rootWords[spell.root];
        Spell c = createSpellFromType(s.type);
        s.copyInto(c);
        int wordCount = 1;
        string[] animData = { null, s.animationID, null };
        string[] sfxData = { null, s.sfxID, null };
        ElementMod e = null;
        StyleMod st = null;
        if (spell.element != null)
        {
            e = elements[spell.element];
            sfxData[2] = e.sfxID;
            animData[2] = e.animationID;
            ++wordCount;
        }
        if (spell.style != null)
        {
            st = styles[spell.style];
            sfxData[0] = st.sfxID;
            animData[0] = st.animationID;
            ++wordCount;
        }
        c.Modify(e, st);
        List<ICaster> toCastAt = c.target(targets, selected, allies, position);
        List<CastData> data = new List<CastData>();
        foreach (ICaster target in toCastAt)
        {
            if (target == null)
                continue;
            if (target.Is_dead)
            {
                noTargetPositions.Add(target.Transform);
                continue;
            }
            CastData castData = c.cast(target, caster);
            animData.CopyTo(castData.animData, 0);
            sfxData.CopyTo(castData.sfxData, 0);
            castData.wordCount = wordCount;
            if (castData.repel == true)
                castData.setLocationData(caster, target);
            else
                castData.setLocationData(target, caster);
            data.Add(castData);
        }
        return data;
    }
    //Will contain method for botching a spell
    public List<CastData> botch(SpellData s, ICaster[] targets, int selected, ICaster[] allies, int position)
    {
        return null;
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
    public ElementMod getElementMod(SpellData data)
    {
        return elements[data.element];
    }
    public StyleMod getStyleMod(SpellData data)
    {
        return styles[data.style];
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
    public Pair<bool[], bool[]> getTargetPattern(SpellData data, ICaster[] targets, int selected, ICaster[] allies, int position)
    {
        if (data.style == null || !styles[data.style].isTarget)
        {
            return rootWords[data.root].targetData.toArrayPair(targets, selected, allies, position);
        }
        else
        {
            TargetData t = new TargetData(false);
            t.copyFrom(rootWords[data.root].targetData);
            styles[data.style].targets.modify(t);
            return t.toArrayPair(targets, selected, allies, position);
        }
    }

    #region SpellBook Management (TO BE RELOCATED)

    //Registeres all keywords in Spelldata s if unregistered
    //Returns bool[elem,root,style] (true if successful register, false if already registered
    //Pre: s is a valid spell
    public bool[] safeRegister(SpellBook spellBook, SpellData s)
    {
        return new bool[] { safeRegister(spellBook, s.element), safeRegister(spellBook, s.root), safeRegister(spellBook, s.style) };
    }
    //Registers individual keyword
    public bool safeRegister(SpellBook spellBook, string word)
    {
        if (string.IsNullOrEmpty(word))
            return false;
        if (spellBook.isNotRegistered(word))
        {
            if (rootWords.ContainsKey(word))
            {
                spellBook.register(word, rootWords[word].type, rootWords[word].description);
                return true;
            }
            else if (elements.ContainsKey(word))
            {
                spellBook.register(word, "element", elements[word].description);
                return true;
            }
            else if (styles.ContainsKey(word))
            {
                spellBook.register(word, "style", styles[word].description);
            }
        }
        return false;
    }

    #endregion

    //Helper method for cloning appropriately typed spells
    private Spell createSpellFromType(string type)
    {
        if (type.CompareTo("attack") == 0)
            return new AttackSpell();
        else if (type.CompareTo("buff") == 0)
            return new BuffSpell();
        else if (type.CompareTo("heal") == 0)
            return new HealSpell();
        else if (type.CompareTo("shield") == 0)
            return new ShieldSpell();
        else
            throw new System.NotImplementedException();
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
        if (element != null)
            result = element + "-" + root;
        else
            result = root;
        if (style != null)
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
    //Gets casting time of input spell
    public float getCastingTime(SpellDictionary dict, float speed)
    {
        float time = dict.getRoot(this).cooldown;
        float baseTime = time;
        if (element != null)
        {
            ElementMod e = dict.getElementMod(this);
            time += e.cooldownMod + (baseTime * e.cooldownModM);
        }
        if (style != null)
        {
            StyleMod s = dict.getStyleMod(this);
            time += s.cooldownMod + (baseTime * s.cooldownModM);
            if (element != null)
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