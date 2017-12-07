using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellDictionary{

    // Use this for initialization
    SpellDictionary(string path)
    { 
        is_loaded = false;
        buildDicts(path); // load gameflow file
        is_loaded = true;
    }

	public bool is_loaded; // is the spellDict done loading?

	TextAsset text_file; // original text asset
	char[] line_delim = { '\n' };
	char[] col_delim = { '\t' };

    //Dictionaries containing spells associated with keywords
    private Dictionary<string, Spell> spells = new Dictionary<string, Spell>();
    private Dictionary<string, ElementMod> elements = new Dictionary<string, ElementMod>();
    private Dictionary<string, StyleMod> styles = new Dictionary<string, StyleMod>();

    // parses Dictionary file which should be a tab-delimited txt file (from excel)
    void buildDicts(string path)
    {
		text_file = Resources.Load<TextAsset> (path);
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
            s.description = cols[2].Trim();
            int.TryParse(cols[3].Trim(), out s.power);
            int.TryParse(cols[4].Trim(), out s.cooldown);
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
            int.TryParse(cols[2].Trim(), out e.cooldownMod);
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
            int.TryParse(cols[2].Trim(), out s.cooldownMod);
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
}
