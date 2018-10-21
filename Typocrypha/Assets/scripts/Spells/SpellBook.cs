using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

//Class that contains and organizes the keyword and description data of the spells known by the player
//IMPROVE PAGE SYSTEM, NEEDS IMPROVEMENT
public class SpellBook : MonoBehaviour
{
    //Private constants
    private const int pageSize = 5;     //Number of entries per page
    private const int entryLength = 2;  //Data fields per entry
    private const string titleString = "SPELLBOOK - ";
    private const string emptyEntryText = "";

    //UI text objects
    public Text pageTitle;
    public GameObject upArrow;
    public GameObject downArrow;
    private Text[] entryNames = new Text[pageSize];
    private Text[] descritptions = new Text[pageSize];

    private int typeIndex = 0;
    private int pageIndex = 0;
    private List<SpellbookList> data = new List<SpellbookList>();
    private Dictionary<string, SpellbookList> typeLog = new Dictionary<string, SpellbookList>();
    private HashSet<string> wordLog = new HashSet<string>();

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < pageSize; i++)
        {
            entryNames[i] = gameObject.transform.GetChild(i).GetComponent<Text>();
            descritptions[i] = gameObject.transform.GetChild(i).GetChild(0).GetComponent<Text>();
            entryNames[i].text = emptyEntryText;
            descritptions[i].text = emptyEntryText;
        }
        updatePage();
    }

    #region Registration
    //Returns true if registered, false if not
    public bool isRegistered(string word)
    {
        return wordLog.Contains(word);
    }
    //Registeres all keywords in Spelldata s if they are unregistered and valid spell words in the given SpellDictionary
    //Returns bool[elem,root,style] (true if successful register, false if already registered)
    public SpellWord[] safeRegister(SpellData s)
    {
        List<SpellWord> ret = new List<SpellWord>();
        SpellWord curr;
        foreach (SpellWord word in s)
            if ((curr = safeRegister(word)) != null)
                ret.Add(word);
        return ret.ToArray();
    }
    //Registers individual keyword if it is unregistered and is a valid spellword in the given dictionary
    public SpellWord safeRegister(SpellWord word)
    {
        if (word == null)
            return null;
        if (!isRegistered(word.name))
        {
            register(word.name, word.Type.ToString(), word.description);
            return word;
        }
        return null;
    }
    //Inserts spell into spellbook (with description string)
    private void register(string word, string type, string description)
    {
        if (typeLog.ContainsKey(type))
        {
            SpellbookList l = typeLog[type];
            l.Add(new SpellbookEntry(word, description));
            wordLog.Add(word);
            l.Sort();
        }
        else
        {
            SpellbookList l = new SpellbookList();
            l.type = type;
            data.Add(l);
            typeLog.Add(type, l);
            l.Add(new SpellbookEntry(word, description));
            wordLog.Add(word);
            l.Sort();
            data.Sort();
        }
        updatePage();
        Debug.Log(word.ToUpper() + " was registered to the spellbook");
    }
    #endregion

    #region Paging and Page Rendering
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
        if (pageIndex - pageSize < 0)
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
        for (int i = 0; i < pageSize; i++)
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
    #endregion

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
