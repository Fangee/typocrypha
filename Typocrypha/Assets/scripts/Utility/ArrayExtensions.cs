using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayExtensions
{
    public static void PopulateWithNew<T>(this T[] arr) where T : new()
    {
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = new T();
        }
    }
}
