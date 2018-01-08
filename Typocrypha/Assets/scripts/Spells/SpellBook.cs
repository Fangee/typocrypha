using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //Build from serialized data
    //Sets current page to 0, and initialized currentPageLength
    public void build(string[,,] words)
    {

    }

    //Goes to next page of spellbook (if one exists)
    //Returns true on success, false on failure (no next page)
    public bool nextPage()
    {
        return false;
    }
    //Goes to last page of spellbook (if one exists)
    //Returns true on success, false on failure (no last page)
    public bool lastPage()
    {
        return false;
    }
    //Goes to page of spellbook at specified index (if one exists)
    //Returns true on success, false on failure (invaleid index)
    public bool setPage(int index)
    {
        return false;
    }
    //Inserts spell into spellbook
    //Safe and efficient to use with spells already in spellbook
    //returns true if success, false if spellbook already contains this keyword
    public bool insert(string word, string type, string description)
    {
        if (log.ContainsKey(word))
            return false;
        data.Add(new SpellbookEntry(word, type, description));
        log.Add(word, true);
        data.Sort();
        return true;
    }
    //NEEDS IMPROVEMENT
    private void updatePageData()
    {
        if (currentPageLength == pageSize)
            return;
        int current = page + currentPageLength;
        while (true)
        {
            if (data[current].type != data[current + 1].type)
                break;
            currentPageLength++;
            current = page + currentPageLength;
        }
    }

    private int page = 0;
    private int currentPageLength;
    private List<SpellbookEntry> data = new List<SpellbookEntry>();
    private Dictionary<string, bool> log = new Dictionary<string, bool>();

    //Private entry class. sorts basen on Compare function
    private class SpellbookEntry : IComparer<SpellbookEntry>
    {
        public SpellbookEntry(string word, string type, string description)
        {
            this.word = word;
            this.type = type;
            this.description = description;
        }
        public string type;
        public string word;
        public string description;

        public int Compare(SpellbookEntry x, SpellbookEntry y)
        {
            throw new System.NotImplementedException();
        }
    }

}
