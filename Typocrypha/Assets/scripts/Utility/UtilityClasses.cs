using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A class that allows integral values to be stored as references in Unity/c# 4
public class Ref<T>
{
    public Ref(T element)
    {
        this.element = element;
    }
    T element;
    public T Obj { get { return element; } set { element = value; } }
}

namespace Utility
{
    public static class Math
    {
        public static int Clamp(int value, int min, int max)
        {
            if (value > max)
                return max;
            if (value < min)
                return min;
            else
                return value;
        }
        public static int ClampCielToInt(float value, int min, int max)
        {
            if (value > max)
                return max;
            if (value < min)
                return min;
            else
                return Mathf.CeilToInt(value);
        }
        public static int ClampFloorToInt(float value, int min, int max)
        {
            if (value > max)
                return max;
            if (value < min)
                return min;
            else
                return Mathf.FloorToInt(value);
        }
    }
    public static class String
    {
        //Returns a version of a string with first letter capitalized
        public static string FirstLetterToUpperCase(this string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new System.ArgumentException("There is no first letter");

            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

    }

}


