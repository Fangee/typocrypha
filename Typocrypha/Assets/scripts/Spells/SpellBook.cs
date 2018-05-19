using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

//Class that contains and organizes the keyword and description data of the spells known by the player
//IMPROVE PAGE SYSTEM, NEEDS IMPROVEMENT
public class SpellBook : MonoBehaviour {

    //UI text objects

    public Text pageTitle;
    public GameObject upArrow;
    public GameObject downArrow;
    private Text[] entryNames = new Text[pageSize];
    private Text[] descritptions = new Text[pageSize];

    //Private constants
    private const int pageSize = 5;     //Number of entries per page
    private const int entryLength = 2;  //Data fields per entry
    private const string titleString = "SPELLBOOK - ";
    private const string emptyEntryText = "";

	// Use this for initialization
	void Start () {
        //Build() or build in loadgameflow or load_file or something
		for(int i = 0; i < pageSize; i++)
        {
            entryNames[i] = gameObject.transform.GetChild(i).GetComponent<Text>();
            descritptions[i] = gameObject.transform.GetChild(i).GetChild(0).GetComponent<Text>();
            entryNames[i].text = emptyEntryText;
            descritptions[i].text = emptyEntryText;
        }
        updatePage();
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
        string add = word;
        //if (type == "element")
        //    add = word + "-";
        //else if (type == "style")
        //    add = "-" + word;
        //else if (type == "friend")
        //    add = word;
        //else
        //    add = "-" + word + "-";
        if (typeLog.ContainsKey(type))
        {
            SpellbookList l = typeLog[type]; 
            l.Add(new SpellbookEntry(add, description));
            wordLog.Add(add, true);
            l.Sort();
        }
        else
        {
            SpellbookList l = new SpellbookList();
            l.type = type;
            data.Add(l);
            typeLog.Add(type, l);
            l.Add(new SpellbookEntry(add, description));
            wordLog.Add(word, true);
            l.Sort();
            data.Sort();
        }
        updatePage();
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
        if (data.Count <= 0)
            return false;
        if (pageIndex + pageSize >= data[typeIndex].Count)
        {
            if (typeIndex + 1 >= data.Count)
                return false;
            typeIndex++;
            pageIndex = 0;
            updatePage();
            return true;
        }
        pageIndex += pageSize;
        updatePage();
        return true;
    }
    //Goes to previous page of spellbook (if one exists)
    //Returns true on success, false on failure (no previous page)
    public bool previousPage()
    {
        if (data.Count <= 0)
            return false;
        if(pageIndex - pageSize < 0)
        {
            if (typeIndex <= 0)
                return false;
            typeIndex--;
            int mod = (data[typeIndex].Count % pageSize);
            if (mod == 0)
                pageIndex = data[typeIndex].Count - pageSize;
            else
                pageIndex = data[typeIndex].Count - mod;
            updatePage();
            return true;
        }
        pageIndex -= pageSize;
        updatePage();
        return true;

    }
    //Updates Render Data
    private bool updatePage()
    {
        if (data.Count == 0)
        {
            pageTitle.text = titleString + "NO SPELLS";
            downArrow.SetActive(false);
            upArrow.SetActive(false);
            return false;
        }
        SpellbookList current = data[typeIndex];
        pageTitle.text = titleString + current.type.ToUpper() + " " + (pageIndex / pageSize + 1);
        int j = pageIndex;
        for(int i = 0; i < pageSize; i++)
        {
            if (j < current.Count)
            {
                entryNames[i].text = current[j].word.ToUpper();
                descritptions[i].text = current[j].description;
            }
            else
            {
                entryNames[i].text = emptyEntryText;
                descritptions[i].text = emptyEntryText;
            }
            ++j;
        }
        downArrow.SetActive(checkNext());
        upArrow.SetActive(checkPrev());
        return true;
    }
    //returns true iff there is a next page
    private bool checkNext()
    {
        if (pageIndex + pageSize >= data[typeIndex].Count && typeIndex + 1 >= data.Count)
            return false;
        return true;
    }
    //returns true iff there is a previous page
    private bool checkPrev()
    {
        if (pageIndex - pageSize < 0 && typeIndex <= 0)
            return false;
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
