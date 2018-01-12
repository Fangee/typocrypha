using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Class that contains and organizes the keyword and description data of the spells known by the player
//IMPROVE PAGE SYSTEM, NEEDS IMPROVEMENT
public class SpellBook : MonoBehaviour {

    //Private constants
    private const int pageSize = 6;     //Number of entries per page
    private const int entryLength = 2;  //Data fields per entry

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }

    //Returns true if registered, false if not
    public bool isNotRegistered(string word)
    {
        if (wordLog.ContainsKey(word))
            return false;
        return true;
    }

    //Inserts spell into spellbook
    //returns true if success, false if spellbook already contains this keyword
    //Pre: this.isRegistered(word) has been called
    public void register(string word, string type, string description)
    {
        if (typeLog.ContainsKey(type))
        {
            SpellbookList l = typeLog[type];
            l.Add(new SpellbookEntry(word, description));
            wordLog.Add(word, true);
            l.Sort();
        }
        else
        {
            SpellbookList l = new SpellbookList();
            l.type = type;
            data.Add(l);
            typeLog.Add(type, l);
            l.Add(new SpellbookEntry(word, description));
            wordLog.Add(word, true);
            l.Sort();
            data.Sort();
        }
        Debug.Log(word.ToUpper() + " was registered to the spellbook");
    }

    //Build from string data (for saving/loading)
    //Sets current page to 0, and initializes currentPageLength
    public void build(string[,,] words)
    {
        throw new System.NotImplementedException();
    }

    //Goes to next page of spellbook (if one exists)
    //Returns true on success, false on failure (no next page)
    public bool nextPage()
    {
        if(pageIndex + pageSize >= data[typeIndex].Count)
        {
            if (typeIndex + 1 >= data.Count)
                return false;
            typeIndex++;
            pageIndex = 0;
            Debug.Log("Now on page " + data[typeIndex].type + " " + pageIndex / pageSize + 1);
            return true;
        }
        pageIndex += pageSize;
        Debug.Log("Now on page " + data[typeIndex].type + " " + pageIndex / pageSize + 1);
        return true;
    }
    //Goes to last page of spellbook (if one exists)
    //Returns true on success, false on failure (no last page)
    public bool lastPage()
    {
        if(pageIndex - pageSize <= 0)
        {
            if (typeIndex <= 0)
                return false;
            typeIndex--;
            pageIndex = (data[typeIndex].Count - (data[typeIndex].Count % pageSize));
            Debug.Log("Now on page " + data[typeIndex].type + " " + pageIndex / pageSize + 1);
            return true;
        }
        pageIndex -= pageSize;
        Debug.Log("Now on page " + data[typeIndex].type + " " + pageIndex / pageSize + 1);
        return true;

    }

    private int typeIndex = 0;
    private int pageIndex = 0;
    private List<SpellbookList> data = new List<SpellbookList>();
    private Dictionary<string, SpellbookList> typeLog = new Dictionary<string, SpellbookList>();
    private Dictionary<string, bool> wordLog = new Dictionary<string, bool>();

    //Private entry class. sorts based on Compare function
    private class SpellbookEntry : IComparable<SpellbookEntry>
    {
        public SpellbookEntry(string word, string description)
        {
            this.word = word;
            this.description = description;
        }
        public string word;
        public string description;

        public int CompareTo(SpellbookEntry other)
        {
            return word.CompareTo(other.word);
        }
    }

    private class SpellbookList : List<SpellbookEntry>, IComparable<SpellbookList>
    {
        public string type = null;

        public int CompareTo(SpellbookList other)
        {
            return type.CompareTo(other.type);
        }
    }
}
