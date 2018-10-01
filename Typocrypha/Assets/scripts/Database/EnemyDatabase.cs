using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Builds and stores a database of enemy stats
public class EnemyDatabase
{
 //   public static EnemyDatabase main = new EnemyDatabase("enemyDatabase");
	//public bool is_loaded = false; // is enemy database loaded?
 //   Dictionary<string, EnemyStats> database = new Dictionary<string, EnemyStats>();

 //   TextAsset text_file; // original text asset
 //   public string file_name; // name of database file
 //   char[] line_delim = { '\n' };
 //   char[] col_delim = { '\t' };

 //   public EnemyDatabase(string path)
 //   {
 //       file_name = path;
 //   }

 //   //Builds database from text file specified by text_file
 //   public void build()
 //   {
 //       text_file = Resources.Load<TextAsset>(file_name);
 //       string[] lines = text_file.text.Split(line_delim);
 //       string[] cols;
 //       //Declare fields
 //       string name, sprite_path, chat_id, ai_type;
 //       int max_hp, max_shield, max_stagger,evade;//declare stat variables
 //       float speed, acc, atk, def;
 //       float[] vsElem;
 //       //For each line in input file
 //       for (int i = 1; lines[i].Trim().CompareTo("END") != 0; i++)
 //       {
 //           //FIELDS//

 //           cols = lines[i].Split(col_delim);
 //           int ind = 0;//Allows new stats to be added without changing the constants
 //           name = cols[ind].Trim();
 //           sprite_path = cols[++ind].Trim();
 //           chat_id = cols[++ind].Trim().ToLower();
 //           int.TryParse(cols[++ind].Trim(), out max_hp);
 //           int.TryParse(cols[++ind].Trim(), out max_shield);
 //           int.TryParse(cols[++ind].Trim(), out max_stagger);
 //           float.TryParse(cols[++ind].Trim(), out atk);
 //           float.TryParse(cols[++ind].Trim(), out def);
 //           float.TryParse(cols[++ind].Trim(), out speed);
 //           float.TryParse(cols[++ind].Trim(), out acc);
 //           int.TryParse(cols[++ind].Trim(), out evade);
 //           ai_type = cols[++ind].Trim();
 //           string[] ai_params = null;
 //           if (cols[++ind].Trim() != "none")
 //               ai_params = cols[ind].Trim('"').Split(',');

 //           //ELEMENTS//

 //           vsElem = new float[Elements.count];
 //           //Read in elemental weakness/resistances
 //           for (int j = ++ind; j < ind + Elements.count; j++)
 //           {
 //               float.TryParse(cols[j].Trim(), out vsElem[j - ind]);
 //           }

 //           //SPELLS//

 //           //Read in Spell List
 //           Dictionary<string, SpellData[]> spellGroups = new Dictionary<string, SpellData[]>();
 //           List<SpellData> spells = null;
 //           string key = "";
 //           for (int j = ind + Elements.count; ; j++)
 //           {
 //               if (cols[j].Trim().CompareTo("END") == 0)
 //               {
 //                   if (spells != null)
 //                   {
 //                       spellGroups.Add(key, spells.ToArray());
 //                   }
 //                   break;
 //               }
 //               if(cols[j].Trim().Contains("GROUP"))
 //               {
 //                   if (spells != null)
 //                   {
 //                       spellGroups.Add(key, spells.ToArray());
 //                   }
 //                   key = cols[j].Trim().Substring(6);
 //                   spells = new List<SpellData>();
 //                   j++;
 //               }
 //               string root = cols[j].Trim();
 //               j++;
 //               string elem = cols[j].Trim();
 //               if (elem.CompareTo("null") == 0)
 //                   elem = null;
 //               j++;
 //               string style = cols[j].Trim();
 //               if (style.CompareTo("null") == 0)
 //                   style = null;
 //               SpellData s = new SpellData(root, elem, style);
 //               spells.Add(s);
 //           }
 //           EnemySpellList spellList = new EnemySpellList(spellGroups);
 //           EnemyStats stats = new EnemyStats(name, chat_id, sprite_path, max_hp, max_shield, max_stagger, atk, def, speed, acc, evade, vsElem, spellList, ai_type, ai_params);
 //           database.Add(stats.name, stats);
 //       }
 //       Debug.Log("Enemy Database Loaded");
	//	is_loaded = true;
 //   }
 //   public EnemyStats getData(string id)
 //   {
 //       if(database.ContainsKey(Utility.String.FirstLetterToUpperCase(id)))
 //           return database[Utility.String.FirstLetterToUpperCase(id)].clone();
 //       throw new KeyNotFoundException(id + " is not in the EnemyDatabase");
 //   }
}
