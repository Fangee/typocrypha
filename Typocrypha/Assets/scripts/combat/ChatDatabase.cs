using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Database for lines said during spellcasting
public class ChatDatabase : MonoBehaviour
{
    //Message to print if no special message is chosen
    public const string defaultMessage = "...";

    public bool is_loaded = false; // is chat database loaded?

    public TextAsset text_file; // original text asset
    char[] line_delim = { '\n' };
    char[] col_delim = { '\t' };

    public void Start()
    {
        build();
    }

    //Build database from input file
    public void build()
    {
        string[] lines = text_file.text.Split(line_delim);
        string[] cols;
        ChatList c = null;
        for (int i = 2; ; i++)
        {
            cols = lines[i].Split(col_delim);
            if(cols[0].Trim() == "END")
            {
                if (c != null)
                    add(c);
                break;
            }
            else if (cols[0].Trim() == "NEW_CATEGORY")//Add category to data, and move on to next category
            {
                if (c != null)
                    add(c);
                c = new ChatList(cols[1].Trim());
            }
            else//Add chat line to current category
            {
                int chance;
                int.TryParse(cols[0].Trim(), out chance);
                c.add(cols[1].Trim().Replace("\"", String.Empty), chance);
            }
        }
        Debug.Log("Done loading chat data");
    }
    //Get a random line out of the specified category
    public string getLine(string category)
    {
        string c = category.ToLower();
        if (data.ContainsKey(c))
        {
            return data[c].getLine();
        }
        throw new KeyNotFoundException("There is no chat category: " + category + " (categories are NOT case-sensitive)");
    }
    //Add ChatList to dictionary. throws a DuplicateKeyException if data already contains a ChatList with c.category
    private void add(ChatList c)
    {
        if (data.ContainsKey(c.category))
            throw new DuplicateKeyException("category " + c.category + " is a duplicate!");
        data.Add(c.category, c);
    }
    //Contains all the actual line data (string = catagory, ChatList = line data)
    private Dictionary<string, ChatList> data = new Dictionary<string, ChatList>();
    //Contains lineData, sorted by chance value.
    //Use getLine to get a random line (according to preset distributions)
    //Use add to add a line with a specific chance of being used
    private class ChatList
    {
        public string category;
        private List<ChatEntry> chat = new List<ChatEntry>();
        //The highest chance num of all the list entries
        private int Highest
        {
            get
            {
                if (chat.Count > 0)
                    return chat[chat.Count - 1].chanceNum;
                else
                    return 0;
            }
        }
        //Construct with category data
        public ChatList(string category)
        {
            this.category = category;
        }
        //Add a new entry to the list, then sort
        //Throws a ChatListException if percentage sum would go over 100%
        public void add(string text, int chance)
        {
            int chanceNum = Highest + chance;
            if (chanceNum > 100)
                throw new ChatListException("ChatList excpetion: line chances add up to higher than 100% in category " + category);
            chat.Add(new ChatEntry(text, chanceNum));
            chat.Sort();
        }
        public string getLine()
        {
            int chance = UnityEngine.Random.Range(1, 100);
            if (chance > Highest)
                return defaultMessage;
            foreach (ChatEntry c in chat)
            {
                if (chance <= c.chanceNum)
                    return c.text;
            }
            throw new Exception("chatList exception: chatList is empty");
        }
        //class modeling entry: implements IComparable<ChatEntry> to enable sorting
        private class ChatEntry : IComparable<ChatEntry>
        {
            public ChatEntry(string text, int chanceNum)
            {
                this.text = text;
                this.chanceNum = chanceNum;
            }
            public string text;
            public int chanceNum;
            public int CompareTo(ChatEntry other)
            {
                return chanceNum.CompareTo(other.chanceNum);
            }
        }
        //Exception class to throw when list percentage sum is over 100%
        private class ChatListException : Exception
        {
            public ChatListException(string message) : base(message) { }
        }
    }
    //Exception class to throw when a duplicate category is added
    private class DuplicateKeyException : Exception
    {
        public DuplicateKeyException(string message) : base(message)
        {

        }
    }
}
