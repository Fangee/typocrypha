using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyDatabase
{
    public static AllyDatabase main = new AllyDatabase("allyDatabase");
    public bool is_loaded = false; // is enemy database loaded?
    Dictionary<string, AllyStats> database = new Dictionary<string, AllyStats>();

    private const int numFields = 10;

    TextAsset text_file; // original text asset
    public string file_name; // name of database file
    char[] line_delim = { '\n' };
    char[] col_delim = { '\t' };

    public AllyDatabase(string path)
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
        string name, chat_id, ID;
        int max_hp, max_shield, max_stagger, evade;//declare stat variables
        float speed, acc, atk, def;
        float[] vsElem;
        //For each line in input file
        for (int i = 1; lines[i].Trim().CompareTo("END") != 0; i++)
        {
            //FIELDS//

            cols = lines[i].Split(col_delim);
            int ind = 0;//Allows new stats to be added without changing the constants
            ID = cols[ind].Trim();
            chat_id = cols[++ind].Trim().ToLower();
            name = cols[++ind].Trim();
            int.TryParse(cols[++ind].Trim(), out max_hp);
            int.TryParse(cols[++ind].Trim(), out max_shield);
            int.TryParse(cols[++ind].Trim(), out max_stagger);
            float.TryParse(cols[++ind].Trim(), out atk);
            float.TryParse(cols[++ind].Trim(), out def);
            float.TryParse(cols[++ind].Trim(), out speed);
            float.TryParse(cols[++ind].Trim(), out acc);
            int.TryParse(cols[++ind].Trim(), out evade);
            vsElem = new float[Elements.count];

            //ELEMENTS//

            //Read in elemental weakness/resistances
            for (int j = numFields; j < numFields + Elements.count; j++)
            {
                float.TryParse(cols[j].Trim(), out vsElem[j - numFields]);
            }

            AllyStats stats = new AllyStats(name, chat_id, max_hp, max_shield, max_stagger, atk, def, speed, acc, evade, vsElem);
            database.Add(ID, stats);
        }
        Debug.Log("Ally Database Loaded");
        is_loaded = true;
    }
    public AllyStats getData(string id)
    {
        return database[id].clone();
    }
}
