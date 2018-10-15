using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Serializable2DMatrix<T> : IEnumerable<T>
{
    [SerializeField] private Row[] _rows;
    public int Rows { get { return _rows.Length; } }
    public int Columns { get { return _rows[0].row.Length; } }

    public Serializable2DMatrix(int rows, int columns)
    {
#if DEBUG
        if (rows <= 0 || columns <= 0)
            throw new System.ArgumentOutOfRangeException("Matrix dimensions must be >= 0");
#endif
        _rows = new Row[rows];
        _rows.PopulateWithNew();
        foreach (Row r in _rows)
            r.row = new T[columns];
    }
    private Serializable2DMatrix(Row[] rows)
    {
        _rows = rows;
    }
    //Warning: shallow copy if reference type!
    public Serializable2DMatrix<T> rotated90()
    {
        List<Row> rotatedRows = new List<Row>();
        for(int i = 0; i < Columns; ++i)
        {
            Row r = new Row() { row = new T[Columns] };
            for (int j = 0; j < Rows; ++j)
            {
                r.row[j] = _rows[j][i];
            }
        }
        return new Serializable2DMatrix<T>(rotatedRows.ToArray());
    }
    //Warning: shallow clone if reference type!
    public Serializable2DMatrix<T> rotated180()
    {
        List<Row> rotatedRows = new List<Row>(_rows);
        rotatedRows.Reverse();
        return new Serializable2DMatrix<T>(rotatedRows.ToArray());
    }

    public T this[int row, int col]
    {
        get
        {
            return _rows[row][col];
        }
        set
        {
            _rows[row][col] = value;
        }
    }
    public T[] this[int row]
    {
        get
        {
            return _rows[row].row;
        }
    }

    #region IEnumarable Implementation
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        foreach (Row r in _rows)
            foreach (T t in r.row)
                yield return t;
    }
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        foreach (Row r in _rows)
            foreach (T t in r.row)
                yield return t;
    }
    #endregion

    [System.Serializable]
    private class Row
    {
        public T[] row;
        public T this[int col]
        {
            get
            {
                return row[col];
            }
            set
            {
                row[col] = value;
            }
        }
    }

}

[System.Serializable] public class BoolMatrix2D : Serializable2DMatrix<bool> { public BoolMatrix2D(int rows, int columns) : base(rows, columns) { } }
[System.Serializable] public class IntMatrix2D : Serializable2DMatrix<bool> { public IntMatrix2D(int rows, int columns) : base(rows, columns) { } }
[System.Serializable] public class FloatMatrix2D : Serializable2DMatrix<bool> { public FloatMatrix2D(int rows, int columns) : base(rows, columns) { } }
