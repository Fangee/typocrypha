using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// enum for how a cast went (successful cast, botched, fizzled, etc)
public enum CastStatus { SUCCESS, BOTCH, FIZZLE, ONCOOLDOWN, COOLDOWNFULL, ALLYSPELL };
public enum WordType { ROOT, STYLE, ELEMENT};

//Stores all the spell info and contains methods to parse and cast spells from player input
//Currently does not actually support player or enemy casting, but has parsing
public class SpellDictionary : MonoBehaviour
{
    public CooldownList cooldown;
    public SpellBook spellBook;
    
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
		text_file = Resources.Load<TextAsset> (file_name);
		string[] lines = text_file.text.Split(line_delim);
        string[] cols;
        int i = 1;
        //read in root keywords
        while(true)
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
            if(!buff.Contains("N"))
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
            spells.Add(key, s);
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
            string pattern = cols[++ind].Trim();
            if (pattern.Contains("N"))
                s.isTarget = false;
            else if (pattern.Contains("A"))
            {
                s.targets = new TargetData(true);
                s.targets.selfCenter = false;
                s.targets.targeted = false;
            }
            else
            {
                s.targets = new TargetData(false);
                if (pattern.Contains("L"))
                    s.targets.enemyL = true;
                if (pattern.Contains("M"))
                    s.targets.enemyM = true;
                if (pattern.Contains("R"))
                    s.targets.enemyR = true;
                if (pattern.Contains("l"))
                    s.targets.allyL = true;
                if (pattern.Contains("m"))
                    s.targets.allyM = true;
                if (pattern.Contains("r"))
                    s.targets.allyR = true;
                if (pattern.Contains("S"))
                    s.targets.selfCenter = true;
                if (pattern.Contains("T"))
                    s.targets.targeted = true;
            }
            styles.Add(key, s);
            i++;
        }
    }

    //parses input spell, and returns an appropriate CastStatus code and a built (possibly null, possibly invalid Spelldata)
	public CastStatus parse(string spell, out SpellData s)
    {
        char[] delim = { ' ' };
        string[] lines = spell.Split(delim);
        CastStatus status;
		if (lines.Length == 1)
        {
			string first = lines [0].Trim ();
			if (spells.ContainsKey (first))
            {
                s = new SpellData(first);
                status = CastStatus.SUCCESS;
			}
            else
            {
				s = new SpellData ("b", null, null);
				status = CastStatus.BOTCH;
			}
		}
        else if (lines.Length == 2)
        {
			string first = lines [0].Trim ();
			string second = lines [1].Trim ();
			if (spells.ContainsKey (first))
            {
				if (styles.ContainsKey (second))
                {
                    s = new SpellData(first, null, second);
                    status = CastStatus.SUCCESS;
				}
                else
                {
					s = new SpellData(first, null, "b");
					status = CastStatus.BOTCH;
				}
			}
            else if (spells.ContainsKey (second))
            {
				if (elements.ContainsKey (first))
                {
                    s = new SpellData(second, first, null);
                    status = CastStatus.SUCCESS;
				}
                else
                {
					s = new SpellData(second, "b", null);
					status = CastStatus.BOTCH;
				}
			}
            else
            {
                s = new SpellData("b", "b", null);
                status = CastStatus.BOTCH;
            }
		}
        else if (lines.Length == 3)
        {
			string elem = lines [0].Trim ();
			string root = lines [1].Trim ();
			string style = lines [2].Trim ();
			if (spells.ContainsKey (root))
            {
				if (elements.ContainsKey (elem))
                {
					if (styles.ContainsKey (style))
                    {
                        s = new SpellData(root, elem, style);
                        status = CastStatus.SUCCESS;
					}
                    else
                    {
						s = new SpellData (root, elem, "b");
						status = CastStatus.BOTCH;
					}
				}
                else if (styles.ContainsKey (style))
                {
					s = new SpellData(root, "b", style);
					status = CastStatus.BOTCH;
				}
                else
                {
					s = new SpellData(root, "b", "b");
					status = CastStatus.BOTCH;
				}
			}
            else if (elements.ContainsKey (elem))
            {
				if (styles.ContainsKey (style))
                {
					s = new SpellData("b", elem, style);
					status = CastStatus.BOTCH;
				}
                else
                {
					s = new SpellData("b", elem, "b");
					status = CastStatus.BOTCH;
				}
			}
            else if (styles.ContainsKey (style))
            {
				s = new SpellData("b", "b", style);
				status = CastStatus.BOTCH;
			}
            else
            {
				s = new SpellData("b", "b", "b");
				status = CastStatus.BOTCH;
			}
		}
        else
        {
			s = new SpellData("FIZZLE");
			status = CastStatus.FIZZLE;
		}
        //Get Cooldown
        if (status == CastStatus.SUCCESS)
        {
            if (cooldown.isFull())
            {
                return CastStatus.COOLDOWNFULL;
            }
            else if (isOnCooldown(s))
            {
                return CastStatus.ONCOOLDOWN;
            }
            else if (spells[s.root].type.Contains(allyString))
            {
                return CastStatus.ALLYSPELL;
            }
            else
                return CastStatus.SUCCESS;
        }
        else
            return status;
    }
    //Casts spell from NPC (enemy or ally)
    public List<CastData> cast(SpellData spell, ICaster[] targets, int selected, ICaster[] allies, int position)
    {
        ICaster caster = allies[position];
        Spell s = spells[spell.root];
        Spell c = createSpellFromType(s.type);
        s.copyInto(c);
        string[] animData = { null, c.animationID, null };
        string[] sfxData = { null, c.sfxID, null };
        ElementMod e = null;
        StyleMod st = null;
        if (spell.element != null)
        {
            e = elements[spell.element];
            sfxData[2] = e.sfxID;
            animData[2] = e.animationID;
        }
        if (spell.style != null)
        {
            st = styles[spell.style];
            sfxData[0] = st.sfxID;
            animData[0] = st.animationID;
        }
        c.Modify(e, st);
        List<ICaster> toCastAt = c.target(targets, selected, allies, position);
        List<CastData> data = new List<CastData>();
        foreach(ICaster target in toCastAt)
        {
            CastData castData = c.cast(target, caster);
            castData.animData = animData;
            castData.sfxData = sfxData;
            if(castData.repel == true)
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
    //Gets casting time of input spell
    public float getCastingTime(SpellData s, float speed)
    {
        float time = spells[s.root].cooldown;
        if (s.element != null && s.style != null)
        {
            float baseTime = time;
            time *= elements[s.element].cooldownModM;
            time += (baseTime * styles[s.style].cooldownModM) - baseTime;
            time += elements[s.element].cooldownMod;
            time += styles[s.style].cooldownMod;
        }
        else if(s.element != null)
        {
            time *= elements[s.element].cooldownModM;
            time += elements[s.element].cooldownMod;
        }
        else if (s.style != null)
        {
            time *= styles[s.style].cooldownModM;
            time += styles[s.style].cooldownMod;
        }
        return time * speed;
    }
    //Starts cooldown of spell
    public void startCooldown(SpellData data, Player caster)
    {
        Spell spell = spells[data.root];
        ElementMod e;
        StyleMod s;
        float cooldownTime = spell.cooldown;
        if (data.element != null && data.style != null)
        {
            e = elements[data.element];
            s = styles[data.style];
            float baseTime = cooldownTime;
            cooldownTime *= e.cooldownModM;
            cooldownTime += (baseTime * s.cooldownModM) - baseTime;
            cooldownTime += e.cooldownMod;
            cooldownTime += s.cooldownMod;
        }
        else if (data.element != null)
        {
            e = elements[data.element];
            cooldownTime *= e.cooldownModM;
            cooldownTime += e.cooldownMod;
        }
        else if (data.style != null)
        {
            s = styles[data.style];
            cooldownTime *= s.cooldownModM;
            cooldownTime += s.cooldownMod;
        }
        spell.startCooldown(cooldown, data.root, caster.Stats.speed * cooldownTime);
    }
    //Return cooldown of spell
    //Pre: spell is on cooldown
    public float getTimeLeft(SpellData data)
    {
        Spell s = spells[data.root];
        return s.TimeLeft;
    }
    //Return the targeting pattern
    public Pair<bool[], bool[]> getTargetPattern(SpellData data, ICaster[] targets, int selected, ICaster[] allies, int position)
    {
        if(data.style == null || !styles[data.style].isTarget)
        {
            return spells[data.root].targetData.toArrayPair(targets, selected, allies, position);
        }
        else
        {
            TargetData t = new TargetData(false);
            t.copyFrom(spells[data.root].targetData);
            t.modify(styles[data.style].targets);
            return t.toArrayPair(targets, selected, allies, position);
        }
    }

    //SPELLBOOK MANAGEMENT

    //Registeres all keywords in Spelldata s if unregistered
    //Returns bool[elem,root,style] (true if successful register, false if already registered
    //Pre: s is a valid spell
    public bool[] safeRegister(SpellData s)
    {
        bool[] results = { false, false, false };
        if(spellBook.isNotRegistered(s.root))
        {
            spellBook.register(s.root, regType(s.root), spells[s.root].description);
            results[1] = true;
        }
        if(s.element != null && spellBook.isNotRegistered(s.element))
        {
            spellBook.register(s.element, "element", elements[s.element].description);
            results[0] = true;
        }
        if (s.style != null && spellBook.isNotRegistered(s.style))
        {
            spellBook.register(s.style, "style", styles[s.style].description);
            results[2] = true;
        }
        return results;
    }
    //Registers individual keyword
    //Pre: s != null
    public bool safeRegister(string word)
    {
        if (spellBook.isNotRegistered(word))
        {
            if (spells.ContainsKey(word))
            {
                spellBook.register(word, regType(word), spells[word].description);
                return true;
            }
            else if (styles.ContainsKey(word))
            {
                spellBook.register(word, "element", elements[word].description);
                return true;
            }
            else if (elements.ContainsKey(word))
            {
                spellBook.register(word, "style", styles[word].description);
            }
        }
        return false;
    }
    //Moves page up in member spellBook
    public bool pageUp()
    {
        return spellBook.previousPage();
    }
    //Moves page down in member spellBook
    public bool pageDown()
    {
        return spellBook.nextPage();
    }

    //PRIVATE//--------------------------------------------------------------------------------------------------------------------------------------------//

    private const int seperatorInd = 7; //Index of '/' seperator in ally spells
    private const string allyString = "friend"; //String before '/' in ally spells

    //Helper method for cloning appropriately typed spells
    private Spell createSpellFromType(string type)
    {
        if (type.Contains(allyString))
            return createSpellFromType(type.Substring(seperatorInd));
        else if (type.CompareTo("attack") == 0)
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
    //Get proper spell type for registry (needed for ally spells)
    private string regType(string root)
    {
        if (spells[root].type.Contains(allyString))
            return allyString;
        return spells[root].type;
    }
    //Returns if spell s is on cooldown
    //Pre: coolDownList is not full
    private bool isOnCooldown(SpellData s)
    {
        Spell spell = spells[s.root];//Get root keyword from dictionary
        if (spell.IsOnCooldown)//Casting fails if root is on cooldown
        {
            return true;
        }
        return false;
    }

    //Dictionaries containing spells associated with keywords
    private Dictionary<string, Spell> spells = new Dictionary<string, Spell>();
    private Dictionary<string, ElementMod> elements = new Dictionary<string, ElementMod>();
    private Dictionary<string, StyleMod> styles = new Dictionary<string, StyleMod>();
}
//A class containing the required data to cast a spell (with defined keyword composition)
//Also contains associated methods like ToString()
public class SpellData
{
    //Make a new spelldata instance with root keyword root, element keyword prefix, and style keyword suffix
    public SpellData(string root, string prefix = null, string suffix = null)
    {
        this.root = root;
        element = prefix;
        style = suffix;
    }
    public string root;
    public string element;
    public string style;
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
}



