using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameflow.MathUtils;

[System.Serializable]
public class Serializable2DMatrix<T> : IEnumerable<T>
{
    [SerializeField] private T[] _data;
    public int Rows { get; }
    public int Columns { get; }

    public Serializable2DMatrix(int rows, int columns)
    {
#if DEBUG
        if (rows <= 0 || columns <= 0)
            throw new System.ArgumentOutOfRangeException("Matrix dimensions must be >= 0");
#endif
        Rows = rows;
        Columns = columns;
        _data = new T[rows * columns];
    }
    private Serializable2DMatrix(T[] data)
    {
        _data = data;
    }
    //Warning: shallow copy if reference type!
    public Serializable2DMatrix<T> rotated90()
    {
        T[] rot = new T[_data.Length];
        _data.CopyTo(rot, 0);
        for (IntRange range = new IntRange(0, Columns - 1); range.max < rot.Length; range.shift(Rows))
            System.Array.Reverse(rot, range.min, Columns);
        return new Serializable2DMatrix<T>(rot);
    }
    //Warning: shallow clone if reference type!
    public Serializable2DMatrix<T> rotated180()
    {
        T[] rot = new T[_data.Length];
        for (IntRange range = new IntRange(0, Columns - 1); range.max < rot.Length; range.shift(Rows))
            System.Array.ConstrainedCopy(_data, range.min, rot, rot.Length - range.max, Columns);
        return new Serializable2DMatrix<T>(rot);
    }

    public T this[int row, int col]
    {
        get
        {
            return _data[(row * Columns) + col];
        }
        set
        {
            _data[(row * Columns) + col] = value;
        }
    }
    public T[] this[int row]
    {
        get
        {
            T[] _row = new T[Rows];
            System.Array.ConstrainedCopy(_data, row * Columns, _row, 0, Rows);
            return _row;
        }
    }

    #region IEnumarable Implementation
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return _data.GetEnumerator();
    }
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return _data.GetEnumerator() as IEnumerator<T>;
    }
    #endregion
}

[System.Serializable] public class BoolMatrix2D : Serializable2DMatrix<bool> { public BoolMatrix2D(int rows, int columns) : base(rows, columns) { } }
[System.Serializable] public class IntMatrix2D : Serializable2DMatrix<bool> { public IntMatrix2D(int rows, int columns) : base(rows, columns) { } }
[System.Serializable] public class FloatMatrix2D : Serializable2DMatrix<bool> { public FloatMatrix2D(int rows, int columns) : base(rows, columns) { } }
