using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ATB2;
// Encapsulates battle info
public class Battlefield : MonoBehaviour
{
    public ICaster Player { get { return casters[1,1]; } }

    //DEBUG FOR TESTING ATB TWEAKS
    public Actor[] actorsToAdd;

    #region Row and List Accessor Properties
    public ICaster[] TopRow { get { return casters[0]; } }
    public ICaster[] BottomRow { get { return casters[1]; } }
    public ICaster[] Enemies { get { return castersLists[ICasterType.ENEMY].ToArray(); } }
    public ICaster[] Allies { get { return castersLists[ICasterType.ALLY].ToArray(); } }
    public List<Actor> Actors { get; } = new List<Actor>();
    #endregion

    #region Data and Representative Lists
    private SpaceMatrix spaces = new SpaceMatrix(2, 3);
    private FieldMatrix casters = new FieldMatrix(2, 3);
    private Dictionary<ICasterType, List<ICaster>> castersLists = new Dictionary<ICasterType, List<ICaster>>
    {
        {ICasterType.PLAYER, new List<ICaster>()},
        {ICasterType.ENEMY, new List<ICaster>()},
        {ICasterType.ALLY, new List<ICaster>()},
    };
    #endregion

    //Initialize space objects from children
    private void Start()
    {
        spaces[0, 0] = transform.Find("Left").transform;
        spaces[0, 1] = transform.Find("Middle").transform;
        spaces[0, 2] = transform.Find("Right").transform;
        spaces[1, 0] = transform.Find("AllyLeft").transform;
        spaces[1, 1] = transform.Find("Player").transform;
        spaces[1, 2] = transform.Find("AllyRight").transform;
        Actors.AddRange(actorsToAdd);
    }

    #region Interface Functions
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
        casters[pos] = toAdd;
        castersLists[toAdd.CasterType].Add(toAdd);
        if (toAdd is Actor) Actors.Add(toAdd as Actor);
    }
    //Add a non-caster actor to the battlefield
    public void AddActor(Actor a)
    {
        Actors.Add(a);
    }
    //Get the position of a battlefield space. The space may be empty
    public Vector3 getSpace(Position pos)
    {
        return spaces[pos].position;
    }
    //Get the caster in a specific space. returns null if the space is empty
    public ICaster getCaster(Position pos)
    {
        return casters[pos];
    }
    //Clear the data and representative lists
    public void clear()
    {
        casters = new FieldMatrix(2, 3);
        foreach (var casterList in castersLists.Values)
            casterList.Clear();
        Actors.Clear();
    }
    #endregion

    public void move(Position moveFrom, Position moveTo)
    {
#if DEBUG
        if (casters[moveTo] != null)
            throw new System.Exception("There is already a caster in the " + moveTo.ToString() + " space. Move failed.");
        if (casters[moveFrom] == null)
            throw new System.Exception("There is no caster in the " + moveFrom.ToString() + " space. Move failed");
#endif
        casters[moveFrom].FieldPos = moveTo;
        casters[moveFrom].WorldPos = spaces[moveTo].position;
        casters[moveTo] = casters[moveFrom];
        casters[moveFrom] = null;
    }

    #region Interrupt Tracking Data (potentially to be moved)
    [HideInInspector] public ICaster lastCaster = null;
    [HideInInspector] public List<CastData> lastCast; // last performed cast action
    [HideInInspector] public SpellData lastSpell; // last performed spell
    [HideInInspector] public SpellWord[] lastRegister; // last spell register status
    #endregion

    private class FieldMatrix : Serializable2DMatrix<ICaster>
    {
        public FieldMatrix(int rows, int columns) : base(rows, columns) { }
        public ICaster this[Position pos]
        {
            get
            {
                return this[pos.Row, pos.Col];
            }
            set
            {
                this[pos.Row, pos.Col] = value;
            }
        }

    }
    private class SpaceMatrix : Serializable2DMatrix<Transform>
    {
        public SpaceMatrix(int rows, int columns) : base(rows, columns) { }
        public Transform this[Position pos]
        {
            get
            {
                return this[pos.Row, pos.Col];
            }
            set
            {
                this[pos.Row, pos.Col] = value;
            }
        }
    }
    public class Position : System.IEquatable<Position>
    {
        int _row;
        public int Row { get { return _row; } set { _row = Mathf.Clamp(0, _row, 2); } }
        int _col;
        public int Col { get { return _col; } set { _col = Mathf.Clamp(0, _col, 2); } }
        public Position(int row, int col)
        {
            _row = row;
            _col = col;
        }

        public void SetIllegalPosition (int row, int col)
        {
            _row = row;
            _col = col;
        }
        public bool Equals(Position other)
        {
            return Row == other.Row && Col == other.Col;
        }
    }
}
