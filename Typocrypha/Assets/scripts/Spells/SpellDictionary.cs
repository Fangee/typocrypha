using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Stores all the spell info and contains methods to parse and cast spells from player input
//Currently does not actually support player or enemy casting, but has parsing
public class SpellDictionary : MonoBehaviour
{
    public CooldownList cooldown;
    
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
            string key = cols[0].Trim(); 
            if (key.CompareTo("END") == 0) //At end of root keywords
            {
                i += 2;//Skip example line
                break;
            }
            string type = cols[1].Trim();
            Spell s = createSpellFromType(type);
            s.type = type;
            s.description = cols[2].Trim();
            int.TryParse(cols[3].Trim(), out s.power);
            float.TryParse(cols[4].Trim(), out s.cooldown);
            int.TryParse(cols[5].Trim(), out s.hitPercentage);
            int.TryParse(cols[6].Trim(), out s.elementEffectMod);
            string pattern = cols[7].Trim();
            if (pattern.Contains("L"))
                s.targets[0] = true;
            if (pattern.Contains("M"))
                s.targets[1] = true;
            if (pattern.Contains("R"))
                s.targets[2] = true;
            if (pattern.Contains("P"))
                s.targets[3] = true;
            if (pattern.Contains("T"))
                s.targets[4] = true;
            spells.Add(key, s);
            i++;
        }
        //read in element keywords
        while (true)
        {
            cols = lines[i].Split(col_delim);
            string key = cols[0].Trim();
            if (key.CompareTo("END") == 0)
            {
                i += 2;//Skip example line
                break;
            }
            ElementMod e = new ElementMod();
            e.element = cols[1].Trim();
            float.TryParse(cols[2].Trim(), out e.cooldownMod);
            elements.Add(key, e);
            i++;
        }
        //Read in casting style keywords
        while (true)
        {
            cols = lines[i].Split(col_delim);
            string key = cols[0].Trim();
            if (key.CompareTo("END") == 0)
            {
                return;
            }
            StyleMod s = new StyleMod();
            int.TryParse(cols[1].Trim(), out s.powerMod);
            float.TryParse(cols[2].Trim(), out s.cooldownMod);
            int.TryParse(cols[3].Trim(), out s.accMod);
            int.TryParse(cols[4].Trim(), out s.statusEffectChanceMod);
            string pattern = cols[7].Trim();
            if (pattern.Contains("L"))
                s.targets[0] = true;
            if (pattern.Contains("M"))
                s.targets[1] = true;
            if (pattern.Contains("R"))
                s.targets[2] = true;
            if (pattern.Contains("P"))
                s.targets[3] = true;
            if (pattern.Contains("T"))
                s.targets[4] = true;
            styles.Add(key, s);
            i++;
        }
    }
    //parses input spell, casts if valid (true), botches if misspelled but structure is valid (true), fizzles if invalid structure (false)
    public bool parseAndCast(string spell, Enemy[] targets, int selected, Player caster)
    {
        char[] delim = { ' ' };
        string[] lines = spell.Split(delim);
		if (lines.Length == 1)
        {
			string first = lines [0].Trim ();
			if (spells.ContainsKey (first)) {
				caster.Last_cast = spell;
                castUnmodified(first, targets, selected, caster);
			} else 
				botch ("b", null, null, caster);
		}
        else if (lines.Length == 2)
        {
			string first = lines [0].Trim ();
			string second = lines [1].Trim ();
			if (spells.ContainsKey (first)) {
				if (styles.ContainsKey (second)) {
					caster.Last_cast = spell;
					cast (first, null, second, targets, selected, caster);
				} else
					botch (first, null, "b", caster);
			} else if (spells.ContainsKey (second)) {
				if (elements.ContainsKey (first)) {
					caster.Last_cast = spell;
					cast (second, first, null, targets, selected, caster);
				} else
					botch ("b", second, null, caster);
			}
		}
        else if (lines.Length == 3)
        {
			string elem = lines [0].Trim ();
			string root = lines [1].Trim ();
			string style = lines [2].Trim ();
			if (spells.ContainsKey (root)) {
				if (elements.ContainsKey (elem)) {
					if (styles.ContainsKey (style)) {
						caster.Last_cast = spell;
						cast (root, elem, style, targets, selected, caster);
					} else
						botch (root, elem, "b", caster);
				} else if (styles.ContainsKey (style)) {
					botch (root, "b", style, caster);
				} else {
					botch (root, "b", "b", caster);
				}
			} else if (elements.ContainsKey (elem)) {
				if (styles.ContainsKey (style))
					botch ("b", elem, style, caster);
				else
					botch ("b", elem, "b", caster);
			} else if (styles.ContainsKey (style))
				botch ("b", "b", style, caster);
			else
				botch ("b", "b", "b", caster);

		} else {
			caster.Last_cast = "ERROR";
			return false;
		}
        return true;
    }
    //Casts enemy spell
    public void enemyCast(Enemy caster, SpellData spell, Enemy[] field, int position, Player target)
    {
        Spell s = spells[spell.root];
        Spell c = createSpellFromType(s.type);
        s.copyInto(c);
        ElementMod e;
        StyleMod st;
        if (spell.element == null)
            e = null;
        else
            e = elements[spell.element];
        if (spell.style == null)
            st = null;
        else
            st = styles[spell.style];
        c.Modify(e, st);
        c.enemyCast(field, position, target);
    }
    //Gets casting time of input spell
    public float getCastingTime(SpellData s, float speed)
    {
        float time = spells[s.root].cooldown;
        if (s.element != null)
            time += elements[s.element].cooldownMod;
        if (s.style != null)
            time += styles[s.style].cooldownMod;
        return time * speed;
    }

    //PRIVATE//--------------------------------------------------------------------------------------------------------------------------------------------//

    //Helper method for casting spells
    //root cannot equal null
    private void cast(string root, string element, string style, Enemy[] targets, int selected, Player caster)
    {
        Spell s = spells[root];//Get root keyword from dictionary
        if (s.IsOnCooldown)//Casting fails if root is on cooldown
        {
            Debug.Log("Cast failed: " + root + " is on cooldown for " + s.TimeLeft + " seconds");
            return;
        }
        if(cooldown.isFull())
        {
            Debug.Log("Cast failed: cooldownList is full!");
            return;
        }
        Spell c = createSpellFromType(s.type);//Create copy to not mutate spell definition (in dict)
        s.copyInto(c);
        ElementMod e;
        StyleMod st;
        if (element == null)//If no element keyword
            e = null;
        else//Get element keyword from element dictionary
            e = elements[element];
        if (style == null)//If no style keyword
            st = null;
        else//Get style keyword from style dictionary
            st = styles[style];
        c.Modify(e, st);//Modify copy with style and/or element keywords (if applicable)
        s.startCooldown(cooldown, root, c.cooldown * caster.Speed);//Start spell cooldown (with modified casting time from copy)
        Debug.Log(root + " is going on cooldown for " + (c.cooldown * caster.Speed) + " seconds");
        c.cast(targets, selected, caster);//Apply actual spell effect

    }
    //Helper method to cast root-only spells
    //root cannot equal null
    private void castUnmodified(string root, Enemy[] targets, int selected, Player caster)
    {
        Spell s = spells[root];//Get root keyword from dictionary
        if (s.IsOnCooldown)//Casting fails if root is on cooldown
        {
            Debug.Log("Cast failed: " + root + " is on cooldown for " + s.TimeLeft + " seconds");
            return;
        }
        if (cooldown.isFull())
        {
            Debug.Log("Cast failed: cooldownList is full!");
            return;
        }
        s.startCooldown(cooldown, root, s.cooldown * caster.Speed);//Start spell cooldown (with modified casting time from copy)
        Debug.Log(root + " is going on cooldown for " + (s.cooldown * caster.Speed) + " seconds");
        s.cast(targets, selected, caster);//Apply actual spell effect
    }
    //Will contain method for botching a spell
    private void botch(string root, string elem, string style, Player caster)
    {
        Debug.Log("Botched cast: " + root + "-" + elem + "-" + style);
		caster.Last_cast = "Botch";
        return;
    }
    //Helper method for cloning appropriately typed spells
    private Spell createSpellFromType(string type)
    {
        if (type.CompareTo("attack") == 0)
            return new AttackSpell();
        else if (type.CompareTo("heal") == 0)
            return new HealSpell();
        else if (type.CompareTo("shield") == 0)
            return new ShieldSpell();
        else
            return null;
    }

    //Dictionaries containing spells associated with keywords
    private Dictionary<string, Spell> spells = new Dictionary<string, Spell>();
    private Dictionary<string, ElementMod> elements = new Dictionary<string, ElementMod>();
    private Dictionary<string, StyleMod> styles = new Dictionary<string, StyleMod>();
}

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
    public override string ToString()
    {
        string result;
        if (element != null)
            result = element + "-" + root;
        else
            result = root;
        if (style != null)
            result += ("-" + style);
        return result;
    }
}

