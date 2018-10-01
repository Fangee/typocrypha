using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATB2;
// Encapsulates battle info
public class Battlefield : MonoBehaviour
{
    public enum Position
    {
        NONE = -2,
        ANY = -1,
        LEFT,
        MIDDLE,
        RIGHT,
        ALLYLEFT,
        PLAYER,
        ALLYRIGHT,
    }

    public ICaster Player { get { return casters[Position.PLAYER]; } }

    //DEBUG FOR TESTING ATB TWEAKS
    public Actor[] actorsToAdd;

    #region Row and List Accessor Properties
    public ICaster[] TopRow { get { return new ICaster[3] { getCaster(Position.LEFT), getCaster(Position.MIDDLE), getCaster(Position.RIGHT) }; } }
    public ICaster[] BottomRow { get { return new ICaster[3] { getCaster(Position.ALLYLEFT), getCaster(Position.PLAYER), getCaster(Position.ALLYRIGHT) }; } }
    public ICaster[] Enemies { get { return castersLists[ICasterType.ENEMY].ToArray(); } }
    public ICaster[] Allies { get { return castersLists[ICasterType.ALLY].ToArray(); } }
    public List<Actor> Actors { get { return actors; } }
    #endregion

    #region Data and Representative Lists
    private Dictionary<Position, Transform> spaces;
    private Dictionary<Position, ICaster> casters = new Dictionary<Position, ICaster>();
    private Dictionary<ICasterType, List<ICaster>> castersLists = new Dictionary<ICasterType, List<ICaster>>
    {
        {ICasterType.PLAYER, new List<ICaster>()},
        {ICasterType.ENEMY, new List<ICaster>()},
        {ICasterType.ALLY, new List<ICaster>()},
    };
    private List<Actor> actors = new List<Actor>();
    #endregion

    //Initialize space objects from children
    private void Start()
    {
        spaces = new Dictionary<Position, Transform>
        {
            {Position.LEFT, transform.Find("Left").transform },
            {Position.MIDDLE, transform.Find("Middle").transform },
            {Position.RIGHT, transform.Find("Right").transform },
            {Position.ALLYLEFT, transform.Find("AllyLeft").transform },
            {Position.PLAYER, transform.Find("Player").transform },
            {Position.ALLYRIGHT, transform.Find("AllyRight").transform },
        };
        actors.AddRange(actorsToAdd);
    }

    #region Dictionary Functions
    //Add a caster to the battlefield at toAdd.FieldPos
    //Implicitly add toAdd to the actor list if applicable
    public void Add(ICaster toAdd)
    {
        Add(toAdd, toAdd.FieldPos);
    }
    //Add a caster to the battlefield at given field position
    //Implicitly add toAdd to the actor list if applicable
    public void Add(ICaster toAdd, Position pos)
    {
        toAdd.FieldPos = pos;
        toAdd.WorldPos = spaces[pos].transform.position;
        casters.Add(pos, toAdd);
        castersLists[toAdd.CasterType].Add(toAdd);
        if (toAdd is Actor) actors.Add(toAdd as Actor);
    }
    //Add a non-caster actor to the battlefield
    public void AddActor(Actor a)
    {
        actors.Add(a);
    }
    //Get the position of a battlefield space. The space may be empty
    public Vector3 getSpace(Position pos)
    {
        return spaces[pos].position;
    }
    //Get the caster in a specific space. returns null if the space is empty
    public ICaster getCaster(Position pos)
    {
        return casters.ContainsKey(pos) ? casters[pos] : null;
    }
    //Clear the data and representative lists
    public void clear()
    {
        casters.Clear();
        foreach (var casterList in castersLists.Values)
            casterList.Clear();
        actors.Clear();
    }
    #endregion

    public void move(Position moveFrom, Position moveTo)
    {
#if DEBUG
        if (getCaster(moveTo) != null)
            throw new System.Exception("There is already a caster in the " + moveTo.ToString() + " space. Move failed.");
        if (getCaster(moveFrom) == null)
            throw new System.Exception("There is no caster in the " + moveFrom.ToString() + " space. Move failed");
#endif
        casters[moveFrom].FieldPos = moveTo;
        casters.Add(moveTo, casters[moveFrom]);
        casters.Remove(moveFrom);
    }

    #region Interrupt Tracking Data (potentially to be moved)
    [HideInInspector] public ICaster lastCaster = null;
    [HideInInspector] public List<CastData> lastCast; // last performed cast action
    [HideInInspector] public SpellData lastSpell; // last performed spell
    [HideInInspector] public bool[] lastRegister; // last spell register status
    #endregion
}
