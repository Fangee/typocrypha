using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Builds and stores a database of enemy stats
public class EnemyDatabase
{
    public static EnemyDatabase main = new EnemyDatabase("enemyDatabase");
	public bool is_loaded = false; // is enemy database loaded?
    Dictionary<string, EnemyStats> database = new Dictionary<string, EnemyStats>();

    private const int numFields = 10;

    TextAsset text_file; // original text asset
    public string file_name; // name of database file
    char[] line_delim = { '\n' };
    char[] col_delim = { '\t' };

    public EnemyDatabase(string path)
    {
        file_name = path;
    }

    //Builds database from text file specified by text_file
    public void build()
    {
        text_file = Resources.Load<TextAsset>(file_name);
        string[] lines = text_file.text.Split(line_delim);
        string[] cols;
        //Declare fields
        string name, sprite_path;
        int max_hp, max_shield, max_stagger,evade;//declare stat variables
        float speed, acc, atk, def;
        float[] vsElem;
        //For each line in input file
        for (int i = 1; lines[i].Trim().CompareTo("END") != 0; i++)
        {
            cols = lines[i].Split(col_delim);
            int ind = 0;//Allows new stats to be added without changing the constants
            name = cols[ind].Trim();
            sprite_path = cols[++ind].Trim();
            int.TryParse(cols[++ind].Trim(), out max_hp);
            int.TryParse(cols[++ind].Trim(), out max_shield);
            int.TryParse(cols[++ind].Trim(), out max_stagger);
            float.TryParse(cols[++ind].Trim(), out atk);
            float.TryParse(cols[++ind].Trim(), out def);
            float.TryParse(cols[++ind].Trim(), out speed);
            float.TryParse(cols[++ind].Trim(), out acc);
            int.TryParse(cols[++ind].Trim(), out evade);
            vsElem = new float[Elements.count];
            //Read in elemental weakness/resistances
            for (int j = numFields; j < numFields + Elements.count; j++)
            {
                float.TryParse(cols[j].Trim(), out vsElem[j - numFields]);
            }
            //Read in Spell List
            List<SpellData> spells = new List<SpellData>();
            for (int j = numFields + Elements.count; cols[j].Trim().CompareTo("END") != 0; j++)
            {
                string root = cols[j].Trim();
                j++;
                string elem = cols[j].Trim();
                if (elem.CompareTo("null") == 0)
                    elem = null;
                j++;
                string style = cols[j].Trim();
                if (style.CompareTo("null") == 0)
                    style = null;
                SpellData s = new SpellData(root, elem, style);
                spells.Add(s);
            }
            EnemyStats stats = new EnemyStats(name, "sprites/" + sprite_path, max_hp, max_shield, max_stagger, atk, def, speed, acc, evade, vsElem, spells.ToArray());
            database.Add(stats.name, stats);
        }
        Debug.Log("Enemy Database Loaded");
		is_loaded = true;
    }
    public EnemyStats getData(string id)
    {
        return database[id];
    }
}
